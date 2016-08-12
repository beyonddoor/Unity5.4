using System;
using System.IO;
using System.Text;

namespace Unity.PackageManager.Ivy
{
	internal class UTF8StringWriter : StringWriter
	{
		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		public UTF8StringWriter(StringBuilder builder) : base(builder)
		{
		}
	}
}
