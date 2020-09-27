using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory;

namespace WpfApp1
{
	class AmongUsScraper
	{
		public AmongUsScraper(string pid)
		{
			m_mem = new Mem();
			bool opened = m_mem.OpenProcess(pid);

			if (!opened)
			{
				throw new Exception("We should have opened an Among Us Process");
			}
		}

		private Mem m_mem;
	}
}
