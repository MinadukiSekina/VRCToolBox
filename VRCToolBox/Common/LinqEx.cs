using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Common
{
    public static class LinqEx
    {
        // reference:https://heinlein.hatenablog.com/entry/2018/08/15/101552
        public static IQueryable<Tsource> WhereIf<Tsource>(this IQueryable<Tsource> source, Expression<Func<Tsource, bool>> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}
