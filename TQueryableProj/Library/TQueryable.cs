using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TQueryable.Library
{
    public interface TQueryable<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableOrdered<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableFiltered<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableGrouped<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableJoined<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableBoolean<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }
    public interface TQueryableUpdated<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableSelected<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

    public interface TQueryableSingle<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }

}
