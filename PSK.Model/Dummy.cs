using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace PSK.Model
{
	public class Dummy
	{
		public virtual int Id { get; set; }
		public virtual string Text { get; set; }
	}

	public class DummyMappings : ClassMap<Dummy>
	{
		public DummyMappings()
		{
			Id(x => x.Id);
			Map(x => x.Text);
		}
	}
}
