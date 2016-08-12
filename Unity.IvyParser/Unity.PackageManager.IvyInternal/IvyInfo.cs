using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Unity.PackageManager.Ivy;

namespace Unity.PackageManager.IvyInternal
{
	[XmlType("info")]
	public class IvyInfo : XmlSerializable
	{
		[XmlAttribute("version")]
		public string Version;

		[XmlAttribute("organisation")]
		public string Organisation;

		[XmlAttribute("module")]
		public string Module;

		[XmlAttribute("revision")]
		public string Revision;

		[XmlAttribute("branch")]
		public string Branch;

		[XmlAttribute("status")]
		public string Status;

		[XmlAttribute("publication")]
		public string Publication;

		[XmlAttribute("packageType", Namespace = "http://ant.apache.org/ivy/extra")]
		public IvyPackageType Type;

		[XmlAttribute("unityVersion", Namespace = "http://ant.apache.org/ivy/extra")]
		public string UnityVersion;

		[DefaultValue(false), XmlAttribute("published", Namespace = "http://ant.apache.org/ivy/extra")]
		public bool Published;

		[XmlAttribute("title", Namespace = "http://ant.apache.org/ivy/extra")]
		public string Title;

		[XmlElement("description")]
		public string Description;

		[XmlIgnore]
		private List<IvyRepository> repositories = new List<IvyRepository>();

		[XmlElement("repository")]
		private List<IvyRepository> xmlRepositories
		{
			get
			{
				if (this.repositories.Count == 0)
				{
					return null;
				}
				return this.repositories;
			}
			set
			{
				if (value == null)
				{
					this.repositories.Clear();
				}
				else
				{
					this.repositories = value;
				}
			}
		}

		[XmlIgnore]
		public List<IvyRepository> Repositories
		{
			get
			{
				return this.repositories;
			}
		}
	}
}
