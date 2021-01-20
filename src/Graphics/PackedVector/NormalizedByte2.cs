#region License

/* MoonWorks - Game Development Framework
 * Copyright 2021 Evan Hemsley
 */

/* Derived from code by Ethan Lee (Copyright 2009-2021).
 * Released under the Microsoft Public License.
 * See fna.LICENSE for details.

 * Derived from code by the Mono.Xna Team (Copyright 2006).
 * Released under the MIT License. See monoxna.LICENSE for details.
 */

#endregion

#region Using Statements
using System;
using MoonWorks.Math;
#endregion

namespace MoonWorks.Graphics
{
	public struct NormalizedByte2 : IPackedVector<ushort>, IEquatable<NormalizedByte2>
	{
		#region Public Properties

		public ushort PackedValue
		{
			get
			{
				return packedValue;
			}
			set
			{
				packedValue = value;
			}
		}

		#endregion

		#region Private Variables

		private ushort packedValue;

		#endregion

		#region Public Constructors

		public NormalizedByte2(Vector2 vector)
		{
			packedValue = Pack(vector.X, vector.Y);
		}

		public NormalizedByte2(float x, float y)
		{
			packedValue = Pack(x, y);
		}

		#endregion

		#region Public Methods

		public Vector2 ToVector2()
		{
			return new Vector2(
				((sbyte) (packedValue & 0xFF)) / 127.0f,
				((sbyte) ((packedValue >> 8) & 0xFF)) / 127.0f
			);
		}

		#endregion

		#region IPackedVector Methods

		void IPackedVector.PackFromVector4(Vector4 vector)
		{
			packedValue = Pack(vector.X, vector.Y);
		}

		Vector4 IPackedVector.ToVector4()
		{
			return new Vector4(ToVector2(), 0.0f, 1.0f);
		}

		#endregion

		#region Public Static Operators and Override Methods

		public static bool operator !=(NormalizedByte2 a, NormalizedByte2 b)
		{
			return a.packedValue != b.packedValue;
		}

		public static bool operator ==(NormalizedByte2 a, NormalizedByte2 b)
		{
			return a.packedValue == b.packedValue;
		}

		public override bool Equals(object obj)
		{
			return (obj is NormalizedByte2) && Equals((NormalizedByte2) obj);
		}

		public bool Equals(NormalizedByte2 other)
		{
			return packedValue == other.packedValue;
		}

		public override int GetHashCode()
		{
			return packedValue.GetHashCode();
		}

		public override string ToString()
		{
			return packedValue.ToString("X");
		}

		#endregion

		#region Private Static Pack Method

		private static ushort Pack(float x, float y)
		{
			int byte2 = (
				((ushort)
					System.Math.Round(MathHelper.Clamp(x, -1.0f, 1.0f) * 127.0f)
				)
			) & 0x00FF;
			int byte1 = (
				((ushort)
					System.Math.Round(MathHelper.Clamp(y, -1.0f, 1.0f) * 127.0f)
				) << 8
			) & 0xFF00;

			return (ushort) (byte2 | byte1);
		}

		#endregion
	}
}
