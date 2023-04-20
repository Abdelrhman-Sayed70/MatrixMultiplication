using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    public static class MatrixMultiplication
    {
        #region YOUR CODE IS HERE

        //Your Code is Here:
        //==================
        /// <summary>
        /// Multiply 2 square matrices in an efficient way [Strassen's Method]
        /// </summary>
        /// <param name="M1">First square matrix</param>
        /// <param name="M2">Second square matrix</param>
        /// <param name="N">Dimension (power of 2)</param>
        /// <returns>Resulting square matrix</returns>
        /// 

        static public int[,] addMatrices(int[,] v1, int[,] v2, int n)
        {
            int[,] ans = new int[n, n];
            Parallel.ForEach(Enumerable.Range(0, n), i => {
                for (int j = 0; j < n; j++)
                {
                    ans[i, j] = v1[i, j] + v2[i, j];
                }
            });
            return ans;
        }

        static public int[,] subtractMatrices(int[,] v1, int[,] v2, int n)
        {
            int[,] ans = new int[n, n];
            Parallel.ForEach(Enumerable.Range(0, n), i => {
                for (int j = 0; j < n; j++)
                {
                    ans[i, j] = v1[i, j] - v2[i, j];
                }
            });
            return ans;
        }

        static public int[,] NaiveMatrixMultiply(int[,] M1, int[,] M2, int N)
        {
            //Parallel.ForEach(Enumerable.Range(0,N), i => { 
            int[,] ans = new int[N, N];
            Parallel.ForEach(Enumerable.Range(0, N), i => {
                for (int k = 0; k < N; k++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        ans[i, j] += M1[i, k] * M2[k, j];
                    }
                }
            });

            return ans;
        }

        static public int[,] multiMatrices(int[,] v1, int[,] v2)
        {
            int matrixSize = v1.GetLength(0);
            int[,] ans = new int[matrixSize, matrixSize];
            if (matrixSize <= 64)
            {
                return NaiveMatrixMultiply(v1, v2, matrixSize);
            }
            int newSize = matrixSize / 2;

            int[,] A11 = new int[newSize, newSize];
            int[,] A12 = new int[newSize, newSize];
            int[,] A21 = new int[newSize, newSize];
            int[,] A22 = new int[newSize, newSize];

            int[,] B11 = new int[newSize, newSize];
            int[,] B12 = new int[newSize, newSize];
            int[,] B21 = new int[newSize, newSize];
            int[,] B22 = new int[newSize, newSize];

            int[,] C11 = new int[newSize, newSize];
            int[,] C12 = new int[newSize, newSize];
            int[,] C21 = new int[newSize, newSize];
            int[,] C22 = new int[newSize, newSize];

            int[,] P1 = new int[newSize, newSize];
            int[,] P2 = new int[newSize, newSize];
            int[,] P3 = new int[newSize, newSize];
            int[,] P4 = new int[newSize, newSize];
            int[,] P5 = new int[newSize, newSize];
            int[,] P6 = new int[newSize, newSize];
            int[,] P7 = new int[newSize, newSize];

            Parallel.ForEach(Enumerable.Range(0, newSize), i => {
                for (int j = 0; j < newSize; j++)
                {
                    A11[i, j] = v1[i, j];
                    A12[i, j] = v1[i, j + newSize];
                    A21[i, j] = v1[i + newSize, j];
                    A22[i, j] = v1[i + newSize, j + newSize];

                    B11[i, j] = v2[i, j];
                    B12[i, j] = v2[i, j + newSize];
                    B21[i, j] = v2[i + newSize, j];
                    B22[i, j] = v2[i + newSize, j + newSize];
                }
            });

            Parallel.Invoke(
            () => P1 = multiMatrices(A11, subtractMatrices(B12, B22, newSize)),
            () => P2 = multiMatrices(addMatrices(A11, A12, newSize), B22));
            Parallel.Invoke(
            () => P3 = multiMatrices(addMatrices(A21, A22, newSize), B11),
            () => P4 = multiMatrices(A22, subtractMatrices(B21, B11, newSize)));
            Parallel.Invoke(
            () => P5 = multiMatrices(addMatrices(A11, A22, newSize), addMatrices(B11, B22, newSize)),
            () => P6 = multiMatrices(subtractMatrices(A12, A22, newSize), addMatrices(B21, B22, newSize)),
            () => P7 = multiMatrices(subtractMatrices(A11, A21, newSize), addMatrices(B11, B12, newSize)));

            Parallel.Invoke(
            () => C11 = addMatrices(subtractMatrices(addMatrices(P5, P4, newSize), P2, newSize), P6, newSize),
            () => C12 = addMatrices(P1, P2, newSize));
            Parallel.Invoke(
            () => C21 = addMatrices(P3, P4, newSize),
            () => C22 = subtractMatrices(subtractMatrices(addMatrices(P5, P1, newSize), P3, newSize), P7, newSize));

            Parallel.ForEach(Enumerable.Range(0, newSize), i => {
                for (int j = 0; j < newSize; j++)
                {
                    ans[i, j] = C11[i, j];
                    ans[i, j + newSize] = C12[i, j];
                    ans[i + newSize, j] = C21[i, j];
                    ans[i + newSize, j + newSize] = C22[i, j];
                }
            });
            return ans;
        }

        static public int[,] MatrixMultiply(int[,] M1, int[,] M2, int N)
        {
            return multiMatrices(M1, M2);
        }
        #endregion
    }
}