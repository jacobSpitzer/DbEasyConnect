using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.TQuery
{
    class FieldAttributes
    {
        /// <summary>
        /// Auto Increment Attribute.<br/>
        /// Auto-increment allows a unique number to be generated automatically when a new record is inserted into a table. 
        /// Often this is the primary key field that we would like to be created automatically every time a new record is inserted.
        /// </summary>
        public class AutoIncrementAttribute : Attribute
        {
        }
    }
}
