using System;
using System.Text.RegularExpressions;

namespace Unity.DataContract
{
	public class PackageVersion : IComparable
	{
		private static readonly string kVersionMatch = "(?<major>\\d+)\\.(?<minor>\\d+)(\\.(?<micro>\\d+))?(\\.?(?<special>.+))?";

		public int major
		{
			get;
			private set;
		}

		public int minor
		{
			get;
			private set;
		}

		public int micro
		{
			get;
			private set;
		}

		public string special
		{
			get;
			private set;
		}

		public string text
		{
			get;
			private set;
		}

		public int parts
		{
			get;
			private set;
		}

		public PackageVersion(string version)
		{
			if (version == null)
			{
				return;
			}
			Match match = Regex.Match(version, PackageVersion.kVersionMatch);
			if (!match.Success)
			{
				throw new ArgumentException("Invalid version: " + version);
			}
			this.major = int.Parse(match.Groups["major"].Value);
			this.minor = int.Parse(match.Groups["minor"].Value);
			this.micro = 0;
			this.special = string.Empty;
			this.parts = 2;
			if (match.Groups["micro"].Success)
			{
				this.micro = int.Parse(match.Groups["micro"].Value);
				this.parts = 3;
			}
			if (match.Groups["special"].Success)
			{
				this.special = match.Groups["special"].Value;
				this.parts = 4;
				if (!this.ValidateSpecial())
				{
					throw new ArgumentException("Invalid version: " + version);
				}
			}
			this.text = version;
		}

		public override string ToString()
		{
			return this.text;
		}

		public int CompareTo(object obj)
		{
			PackageVersion z = obj as PackageVersion;
			if (this > z)
			{
				return 1;
			}
			if (this == z)
			{
				return 0;
			}
			return -1;
		}

		public override int GetHashCode()
		{
			if (this.text == null)
			{
				return 0;
			}
			return this.text.GetHashCode();
		}

		public bool IsCompatibleWith(PackageVersion other)
		{
			return !(other == null) && (this == other || (this.parts == 2 && other.parts > 2 && this.major == other.major && this.minor == other.minor) || (this.parts == 3 && other.parts >= 3 && this.major == other.major && this.minor == other.minor && this.micro == other.micro));
		}

		public override bool Equals(object obj)
		{
			PackageVersion z = obj as PackageVersion;
			return this == z;
		}

		private bool ValidateSpecial()
		{
			int num = 0;
			int num2;
			while (num < this.special.Length && (num2 = PackageVersion.FindFirstDigit(this.special.Substring(num))) >= 0)
			{
				num += num2;
				int num3 = PackageVersion.FindFirstNonDigit(this.special.Substring(num));
				if (num3 < 0)
				{
					num3 = this.special.Length - num;
				}
				if (int.Parse(this.special.Substring(num, num3)) == 0)
				{
					return false;
				}
				num += num3;
			}
			return true;
		}

		private static int FindFirstNonDigit(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!char.IsDigit(str[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private static int FindFirstDigit(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (char.IsDigit(str[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool operator ==(PackageVersion a, PackageVersion z)
		{
			return (a == null && z == null) || (a != null && z != null && (a.major == z.major && a.minor == z.minor && a.micro == z.micro) && a.special == z.special);
		}

		public static bool operator !=(PackageVersion a, PackageVersion z)
		{
			return !(a == z);
		}

		public static bool operator >(PackageVersion a, PackageVersion z)
		{
			if (a == null && z == null)
			{
				return false;
			}
			if (a == null)
			{
				return false;
			}
			if (z == null)
			{
				return true;
			}
			if (a == z)
			{
				return false;
			}
			if (a.major != z.major)
			{
				return a.major > z.major;
			}
			if (a.minor != z.minor)
			{
				return a.minor > z.minor;
			}
			if (a.micro != z.micro)
			{
				return a.micro > z.micro;
			}
			if (a.parts == z.parts)
			{
				int num = 0;
				int num2 = 0;
				while (num < a.special.Length && num2 < z.special.Length)
				{
					while (num < a.special.Length && num2 < z.special.Length && !char.IsDigit(a.special[num]) && !char.IsDigit(z.special[num2]))
					{
						if (a.special[num] != z.special[num2])
						{
							return a.special[num] > z.special[num2];
						}
						num++;
						num2++;
					}
					if (num < a.special.Length && num2 < z.special.Length && (!char.IsDigit(a.special[num]) || !char.IsDigit(z.special[num2])))
					{
						return char.IsDigit(a.special[num]);
					}
					int num3 = PackageVersion.FindFirstNonDigit(a.special.Substring(num));
					int num4 = PackageVersion.FindFirstNonDigit(z.special.Substring(num2));
					int num5 = -1;
					if (num3 > -1)
					{
						num5 = int.Parse(a.special.Substring(num, num3));
						num += num3;
					}
					else
					{
						int.TryParse(a.special.Substring(num), out num5);
						num = a.special.Length;
					}
					int num6 = -1;
					if (num4 > -1)
					{
						num6 = int.Parse(z.special.Substring(num2, num4));
						num2 += num4;
					}
					else
					{
						int.TryParse(z.special.Substring(num2), out num6);
						num2 = z.special.Length;
					}
					if (num5 != num6)
					{
						return num5 > num6;
					}
				}
				return a.special.Length < z.special.Length;
			}
			if (a.parts == 4)
			{
				return char.IsDigit(a.special[0]);
			}
			return !char.IsDigit(z.special[0]);
		}

		public static bool operator <(PackageVersion a, PackageVersion z)
		{
			return a != z && !(a > z);
		}

		public static bool operator >=(PackageVersion a, PackageVersion z)
		{
			return a == z || a > z;
		}

		public static bool operator <=(PackageVersion a, PackageVersion z)
		{
			return a == z || a < z;
		}

		public static implicit operator string(PackageVersion version)
		{
			if (version == null)
			{
				return null;
			}
			return version.ToString();
		}
	}
}
