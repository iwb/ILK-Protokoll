using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.DataLayer
{
	public class MockupInitializer : DropCreateDatabaseAlways<DataContext>
	{
	}
}