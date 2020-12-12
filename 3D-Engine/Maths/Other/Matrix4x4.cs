﻿/*
 *       -3D-Engine-
 *     (c) Josh Bryant
 * https://joshdbryant.com
 *
 * Full license is available in the GitHub repository:
 * https://github.com/JoshB82/3D-Engine/blob/master/LICENSE
 *
 * Code description for this file:
 * Encapsulates creation of a 4x4 square matrix and provides methods to extract common information and for operator overloading. Each instance of a Matrix4x4 has a size of 64 bytes, so, where possible, a Matrix4x4 should be passed by reference to reduce unnecessary copying.
 */

using System;

namespace _3D_Engine
{
    /// <include file="Help_8.xml" path="doc/members/member[@name='T:_3D_Engine.Matrix4x4']/*"/>
    public struct Matrix4x4 : IEquatable<Matrix4x4>
    {
        #region Fields and Properties

        // Common matrices
        /// <summary>
        /// A <see cref="Matrix4x4"/> with all values set to zero.
        /// </summary>
        public static readonly Matrix4x4 Zero = new Matrix4x4();
        /// <summary>
        /// A <see cref="Matrix4x4"/> equal to the 4x4 identity matrix.
        /// </summary>
        public static readonly Matrix4x4 Identity = new Matrix4x4 { m00 = 1, m11 = 1, m22 = 1, m33 = 1 };

        // Matrix contents
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m00']/*"/>
        public float m00;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m01']/*"/>
        public float m01;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m02']/*"/>
        public float m02;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m03']/*"/>
        public float m03;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m10']/*"/>
        public float m10;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m11']/*"/>
        public float m11;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m12']/*"/>
        public float m12;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m13']/*"/>
        public float m13;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m20']/*"/>
        public float m20;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m21']/*"/>
        public float m21;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m22']/*"/>
        public float m22;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m23']/*"/>
        public float m23;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m30']/*"/>
        public float m30;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m31']/*"/>
        public float m31;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m32']/*"/>
        public float m32;
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Matrix4x4.m33']/*"/>
        public float m33;

        #endregion

        #region Constructors

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.#ctor(System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)']/*"/>
        public Matrix4x4(
            float m00, float m01, float m02, float m03, 
            float m10, float m11, float m12, float m13, 
            float m20, float m21, float m22, float m23, 
            float m30, float m31, float m32, float m33)
        {
            (this.m00, this.m01, this.m02, this.m03) = (m00, m01, m02, m03);
            (this.m10, this.m11, this.m12, this.m13) = (m10, m11, m12, m13);
            (this.m20, this.m21, this.m22, this.m23) = (m20, m21, m22, m23);
            (this.m30, this.m31, this.m32, this.m33) = (m30, m31, m32, m33);
        }

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.#ctor(System.Single[0:,0:])']/*"/>
        public Matrix4x4(float[,] elements)
        {
            if (elements.GetLength(0) < 4 ||
                elements.GetLength(1) < 4)
                throw new ArgumentException("Parameter \"elements\" must at least be of size 4x4.", nameof(elements));

            (m00, m01, m02, m03) = (elements[0, 0], elements[0, 1], elements[0, 2], elements[0, 3]);
            (m10, m11, m12, m13) = (elements[1, 0], elements[1, 1], elements[1, 2], elements[1, 3]);
            (m20, m21, m22, m23) = (elements[2, 0], elements[2, 1], elements[2, 2], elements[2, 3]);
            (m30, m31, m32, m33) = (elements[3, 0], elements[3, 1], elements[3, 2], elements[3, 3]);
        }

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.#ctor(System.Single[][])']/*"/>
        public Matrix4x4(float[][] elements)
        {
            if (elements.Length < 4 ||
                elements[0].Length < 4 ||
                elements[1].Length < 4 ||
                elements[2].Length < 4 ||
                elements[3].Length < 4)
                throw new ArgumentException("Parameter \"elements\" must at least be of size 4x4.", nameof(elements));

            (m00, m01, m02, m03) = (elements[0][0], elements[0][1], elements[0][2], elements[0][3]);
            (m10, m11, m12, m13) = (elements[1][0], elements[1][1], elements[1][2], elements[1][3]);
            (m20, m21, m22, m23) = (elements[2][0], elements[2][1], elements[2][2], elements[2][3]);
            (m30, m31, m32, m33) = (elements[3][0], elements[3][1], elements[3][2], elements[3][3]);
        }

        #endregion

        #region Matrix Operations

        // Common
        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.Determinant']/*"/> // source!
        public readonly float Determinant()
        {
            float d1 = m20 * m31 - m21 * m30;
            float d2 = m20 * m32 - m22 * m30;
            float d3 = m20 * m33 - m23 * m30;
            float d4 = m21 * m32 - m22 * m31;
            float d5 = m21 * m33 - m23 * m31;
            float d6 = m22 * m33 - m23 * m32;

            return
              m00 * (m11 * d6 - m12 * d5 + m13 * d4)
            - m01 * (m10 * d6 - m12 * d3 + m13 * d2)
            + m02 * (m10 * d5 - m11 * d3 + m13 * d1)
            - m03 * (m10 * d4 - m11 * d2 + m12 * d1);
        }

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.Inverse']/*"/>
        public readonly Matrix4x4 Inverse()
        {
            float d1 = m10 * m21 - m11 * m20;
            float d2 = m10 * m22 - m12 * m20;
            float d3 = m10 * m23 - m13 * m20;
            float d4 = m10 * m31 - m11 * m30;
            float d5 = m10 * m32 - m12 * m30;
            float d6 = m10 * m33 - m13 * m30;
            float d7 = m11 * m22 - m12 * m21;
            float d8 = m11 * m23 - m13 * m21;
            float d9 = m11 * m32 - m12 * m31;
            float d10 = m11 * m33 - m13 * m31;
            float d11 = m12 * m23 - m13 * m22;
            float d12 = m12 * m33 - m13 * m32;
            float d13 = m20 * m31 - m21 * m30;
            float d14 = m20 * m32 - m22 * m30;
            float d15 = m20 * m33 - m23 * m30;
            float d16 = m21 * m32 - m22 * m31;
            float d17 = m21 * m33 - m23 * m31;
            float d18 = m22 * m33 - m23 * m32;

            float det =   m00 * (m11 * d18 - m12 * d17 + m13 * d16) 
                        - m01 * (m10 * d18 - m12 * d15 + m13 * d14)
                        + m02 * (m10 * d17 - m11 * d15 + m13 * d13)
                        - m03 * (m10 * d16 - m11 * d14 + m12 * d13);
            if (det == 0) throw new InvalidOperationException("Matrix4x4 does not have an inverse.");

            return new Matrix4x4
            (
                  m11 * d18 - m12 * d17 + m13 * d16,
                -(m01 * d18 - m02 * d17 + m03 * d16),
                  m01 * d12 - m02 * d10 + m03 * d9,
                -(m01 * d11 - m02 * d8 + m03 * d7),
                -(m10 * d18 - m12 * d15 + m13 * d14),
                  m00 * d18 - m02 * d15 + m03 * d14,
                -(m00 * d12 - m02 * d6 + m03 * d5),
                  m00 * d11 - m02 * d3 + m03 * d2,
                  m10 * d17 - m11 * d15 + m13 * d13,
                -(m00 * d17 - m01 * d15 + m03 * d13),
                  m00 * d10 - m01 * d6 + m03 * d4,
                -(m00 * d8 - m01 * d3 + m03 * d1),
                -(m10 * d16 - m11 * d14 + m12 * d13),
                  m00 * d16 - m01 * d14 + m02 * d13,
                -(m00 * d9 - m01 * d5 + m02 * d4),
                  m00 * d7 - m01 * d2 + m02 * d1
            ) / det;
        }

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.Transpose']/*"/>
        public readonly Matrix4x4 Transpose() =>
            new Matrix4x4
            (
                m00, m10, m20, m30,
                m01, m11, m21, m31,
                m02, m12, m22, m32,
                m03, m13, m23, m33
            );

        // Equality and miscellaneous
        public static bool operator ==(Matrix4x4 v1, Matrix4x4 v2) =>
            v1.m00 == v2.m00 && v1.m01 == v2.m01 && v1.m02 == v2.m02 && v1.m03 == v2.m03 &&
            v1.m10 == v2.m10 && v1.m11 == v2.m11 && v1.m12 == v2.m12 && v1.m13 == v2.m13 &&
            v1.m20 == v2.m20 && v1.m21 == v2.m21 && v1.m22 == v2.m22 && v1.m23 == v2.m23 &&
            v1.m30 == v2.m30 && v1.m31 == v2.m31 && v1.m32 == v2.m32 && v1.m33 == v2.m33;

        public static bool operator !=(Matrix4x4 v1, Matrix4x4 v2) => !(v1 == v2);

        public readonly bool Equals(Matrix4x4 m) => this == m;

        public readonly bool Approx_Equals(Matrix4x4 m, float epsilon = float.Epsilon) =>
            this.m00.Approx_Equals(m.m00, epsilon) &&
            this.m01.Approx_Equals(m.m01, epsilon) &&
            this.m02.Approx_Equals(m.m02, epsilon) &&
            this.m03.Approx_Equals(m.m03, epsilon) &&
            this.m10.Approx_Equals(m.m10, epsilon) &&
            this.m11.Approx_Equals(m.m11, epsilon) &&
            this.m12.Approx_Equals(m.m12, epsilon) &&
            this.m13.Approx_Equals(m.m13, epsilon) &&
            this.m20.Approx_Equals(m.m20, epsilon) &&
            this.m21.Approx_Equals(m.m21, epsilon) &&
            this.m22.Approx_Equals(m.m22, epsilon) &&
            this.m23.Approx_Equals(m.m23, epsilon) &&
            this.m30.Approx_Equals(m.m30, epsilon) &&
            this.m31.Approx_Equals(m.m31, epsilon) &&
            this.m32.Approx_Equals(m.m32, epsilon) &&
            this.m33.Approx_Equals(m.m33, epsilon);

        public override readonly bool Equals(object obj) => this == (Matrix4x4)obj;

        public override int GetHashCode() => throw new NotImplementedException();

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.ToString']/*"/>
        public override readonly string ToString() =>
            $"({m00}, {m01}, {m02}, {m03}, \n" +
            $"{m10}, {m11}, {m12}, {m13}, \n" +
            $"{m20}, {m21}, {m22}, {m23}, \n" +
            $"{m30}, {m31}, {m32}, {m33})";

        #endregion

        #region Operator Overloading

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.op_Addition(_3D_Engine.Matrix4x4,_3D_Engine.Matrix4x4)']/*"/>
        public static Matrix4x4 operator +(Matrix4x4 m1, Matrix4x4 m2) =>
            new Matrix4x4()
            {
                m00 = m1.m00 + m2.m00, m01 = m1.m01 + m2.m01, m02 = m1.m02 + m2.m02, m03 = m1.m03 + m2.m03,
                m10 = m1.m10 + m2.m10, m11 = m1.m11 + m2.m11, m12 = m1.m12 + m2.m12, m13 = m1.m13 + m2.m13,
                m20 = m1.m20 + m2.m20, m21 = m1.m21 + m2.m21, m22 = m1.m22 + m2.m22, m23 = m1.m23 + m2.m23,
                m30 = m1.m30 + m2.m30, m31 = m1.m31 + m2.m31, m32 = m1.m32 + m2.m32, m33 = m1.m33 + m2.m33
            };

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.op_Subtraction(_3D_Engine.Matrix4x4,_3D_Engine.Matrix4x4)']/*"/>
        public static Matrix4x4 operator -(Matrix4x4 m1, Matrix4x4 m2) =>
            new Matrix4x4()
            {
                m00 = m1.m00 - m2.m00, m01 = m1.m01 - m2.m01, m02 = m1.m02 - m2.m02, m03 = m1.m03 - m2.m03,
                m10 = m1.m10 - m2.m10, m11 = m1.m11 - m2.m11, m12 = m1.m12 - m2.m12, m13 = m1.m13 - m2.m13,
                m20 = m1.m20 - m2.m20, m21 = m1.m21 - m2.m21, m22 = m1.m22 - m2.m22, m23 = m1.m23 - m2.m23,
                m30 = m1.m30 - m2.m30, m31 = m1.m31 - m2.m31, m32 = m1.m32 - m2.m32, m33 = m1.m33 - m2.m33
            };

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.op_Multiply(_3D_Engine.Matrix4x4,_3D_Engine.Matrix4x4)']/*"/>
        public static Matrix4x4 operator *(Matrix4x4 m1, Matrix4x4 m2) =>
            new Matrix4x4
            (
                m1.m00 * m2.m00 + m1.m01 * m2.m10 + m1.m02 * m2.m20 + m1.m03 * m2.m30,
                m1.m00 * m2.m01 + m1.m01 * m2.m11 + m1.m02 * m2.m21 + m1.m03 * m2.m31,
                m1.m00 * m2.m02 + m1.m01 * m2.m12 + m1.m02 * m2.m22 + m1.m03 * m2.m32,
                m1.m00 * m2.m03 + m1.m01 * m2.m13 + m1.m02 * m2.m23 + m1.m03 * m2.m33,
                m1.m10 * m2.m00 + m1.m11 * m2.m10 + m1.m12 * m2.m20 + m1.m13 * m2.m30,
                m1.m10 * m2.m01 + m1.m11 * m2.m11 + m1.m12 * m2.m21 + m1.m13 * m2.m31,
                m1.m10 * m2.m02 + m1.m11 * m2.m12 + m1.m12 * m2.m22 + m1.m13 * m2.m32,
                m1.m10 * m2.m03 + m1.m11 * m2.m13 + m1.m12 * m2.m23 + m1.m13 * m2.m33,
                m1.m20 * m2.m00 + m1.m21 * m2.m10 + m1.m22 * m2.m20 + m1.m23 * m2.m30,
                m1.m20 * m2.m01 + m1.m21 * m2.m11 + m1.m22 * m2.m21 + m1.m23 * m2.m31,
                m1.m20 * m2.m02 + m1.m21 * m2.m12 + m1.m22 * m2.m22 + m1.m23 * m2.m32,
                m1.m20 * m2.m03 + m1.m21 * m2.m13 + m1.m22 * m2.m23 + m1.m23 * m2.m33,
                m1.m30 * m2.m00 + m1.m31 * m2.m10 + m1.m32 * m2.m20 + m1.m33 * m2.m30,
                m1.m30 * m2.m01 + m1.m31 * m2.m11 + m1.m32 * m2.m21 + m1.m33 * m2.m31,
                m1.m30 * m2.m02 + m1.m31 * m2.m12 + m1.m32 * m2.m22 + m1.m33 * m2.m32,
                m1.m30 * m2.m03 + m1.m31 * m2.m13 + m1.m32 * m2.m23 + m1.m33 * m2.m33
            );

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.op_Multiply(_3D_Engine.Matrix4x4,_3D_Engine.Vector4D)']/*"/>
        public static Vector4D operator *(Matrix4x4 m, Vector4D v) => 
            new Vector4D(
                m.m00 * v.x + m.m01 * v.y + m.m02 * v.z + m.m03 * v.w,
                m.m10 * v.x + m.m11 * v.y + m.m12 * v.z + m.m13 * v.w,
                m.m20 * v.x + m.m21 * v.y + m.m22 * v.z + m.m23 * v.w,
                m.m30 * v.x + m.m31 * v.y + m.m32 * v.z + m.m33 * v.w
            );

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.op_Multiply(_3D_Engine.Matrix4x4,System.Single)']/*"/>
        public static Matrix4x4 operator *(Matrix4x4 m, float scalar) =>
            new Matrix4x4()
            {
                m00 = m.m00 * scalar, m01 = m.m01 * scalar, m02 = m.m02 * scalar, m03 = m.m03 * scalar,
                m10 = m.m10 * scalar, m11 = m.m11 * scalar, m12 = m.m12 * scalar, m13 = m.m13 * scalar,
                m20 = m.m20 * scalar, m21 = m.m21 * scalar, m22 = m.m22 * scalar, m23 = m.m23 * scalar,
                m30 = m.m30 * scalar, m31 = m.m31 * scalar, m32 = m.m32 * scalar, m33 = m.m33 * scalar
            };

        /// <summary>
        /// Finds the product of a scalar and a <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="scalar">The scalar to be multiplied.</param>
        /// <param name="m">The <see cref="Matrix4x4"/> to be multiplied.</param>
        /// <returns>The product of a scalar and a <see cref="Matrix4x4"/>.</returns>
        public static Matrix4x4 operator *(float scalar, Matrix4x4 m) => m * scalar;

        /// <include file="Help_8.xml" path="doc/members/member[@name='M:_3D_Engine.Matrix4x4.op_Division(_3D_Engine.Matrix4x4,System.Single)']/*"/>
        public static Matrix4x4 operator /(Matrix4x4 m, float scalar) =>
            new Matrix4x4()
            {
                m00 = m.m00 / scalar, m01 = m.m01 / scalar, m02 = m.m02 / scalar, m03 = m.m03 / scalar,
                m10 = m.m10 / scalar, m11 = m.m11 / scalar, m12 = m.m12 / scalar, m13 = m.m13 / scalar,
                m20 = m.m20 / scalar, m21 = m.m21 / scalar, m22 = m.m22 / scalar, m23 = m.m23 / scalar,
                m30 = m.m30 / scalar, m31 = m.m31 / scalar, m32 = m.m32 / scalar, m33 = m.m33 / scalar
            };

        #endregion
    }
}