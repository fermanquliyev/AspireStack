using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Repository
{
    public interface IAsyncQueryableExecuter
    {
        #region Contains

        Task<bool> ContainsAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] T item,
            CancellationToken cancellationToken = default);

        Task<bool> ContainsAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] T item,
            CancellationToken cancellationToken = default) => ContainsAsync(enumerable.AsQueryable(), item, cancellationToken);

        #endregion

        #region Any/All

        Task<bool> AnyAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => AnyAsync(enumerable.AsQueryable(), cancellationToken);

        Task<bool> AnyAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => AnyAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        Task<bool> AllAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> AllAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => AllAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        #endregion

        #region Count/LongCount

        Task<int> CountAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => CountAsync(enumerable.AsQueryable(), cancellationToken);

        Task<int> CountAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => CountAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        Task<long> LongCountAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<long> LongCountAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<long> LongCountAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => LongCountAsync(enumerable.AsQueryable(), cancellationToken);

        Task<long> LongCountAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => LongCountAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        #endregion

        #region First/FirstOrDefault

        Task<T> FirstAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<T> FirstAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T> FirstAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => FirstAsync(enumerable.AsQueryable(), cancellationToken);

        Task<T> FirstAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => FirstAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        Task<T?> FirstOrDefaultAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<T?> FirstOrDefaultAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T?> FirstOrDefaultAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => FirstOrDefaultAsync(enumerable.AsQueryable(), cancellationToken);

        Task<T?> FirstOrDefaultAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => FirstOrDefaultAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        #endregion

        #region Last/LastOrDefault

        Task<T> LastAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<T> LastAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T> LastAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => LastAsync(enumerable.AsQueryable(), cancellationToken);

        Task<T> LastAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => LastAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        Task<T?> LastOrDefaultAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<T?> LastOrDefaultAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T?> LastOrDefaultAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => LastOrDefaultAsync(enumerable.AsQueryable(), cancellationToken);

        Task<T?> LastOrDefaultAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => LastOrDefaultAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        #endregion

        #region Single/SingleOrDefault

        Task<T> SingleAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<T> SingleAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T> SingleAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => SingleAsync(enumerable.AsQueryable(), cancellationToken);

        Task<T> SingleAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => SingleAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        Task<T?> SingleOrDefaultAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<T?> SingleOrDefaultAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<T?> SingleOrDefaultAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => SingleOrDefaultAsync(enumerable.AsQueryable(), cancellationToken);

        Task<T?> SingleOrDefaultAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default) => SingleOrDefaultAsync(enumerable.AsQueryable(), predicate, cancellationToken);

        #endregion

        #region Min

        Task<T> MinAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<TResult> MinAsync<T, TResult>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default);

        Task<T> MinAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => MinAsync(enumerable.AsQueryable(), cancellationToken);

        Task<TResult> MinAsync<T, TResult>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default) => MinAsync(enumerable.AsQueryable(), selector, cancellationToken);

        #endregion

        #region Max

        Task<T> MaxAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<TResult> MaxAsync<T, TResult>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default);

        Task<T> MaxAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => MaxAsync(enumerable.AsQueryable(), cancellationToken);

        Task<TResult> MaxAsync<T, TResult>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default) => MaxAsync(enumerable.AsQueryable(), selector, cancellationToken);

        #endregion

        #region Sum

        Task<decimal> SumAsync(
            [NotNull] IQueryable<decimal> source,
            CancellationToken cancellationToken = default);

        Task<decimal?> SumAsync(
            [NotNull] IQueryable<decimal?> source,
            CancellationToken cancellationToken = default);

        Task<decimal> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, decimal>> selector,
            CancellationToken cancellationToken = default);

        Task<decimal?> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, decimal?>> selector,
            CancellationToken cancellationToken = default);

        Task<int> SumAsync(
            [NotNull] IQueryable<int> source,
            CancellationToken cancellationToken = default);

        Task<int?> SumAsync(
            [NotNull] IQueryable<int?> source,
            CancellationToken cancellationToken = default);

        Task<int> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, int>> selector,
            CancellationToken cancellationToken = default);

        Task<int?> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, int?>> selector,
            CancellationToken cancellationToken = default);

        Task<long> SumAsync(
            [NotNull] IQueryable<long> source,
            CancellationToken cancellationToken = default);

        Task<long?> SumAsync(
            [NotNull] IQueryable<long?> source,
            CancellationToken cancellationToken = default);

        Task<long> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, long>> selector,
            CancellationToken cancellationToken = default);

        Task<long?> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, long?>> selector,
            CancellationToken cancellationToken = default);

        Task<double> SumAsync(
            [NotNull] IQueryable<double> source,
            CancellationToken cancellationToken = default);

        Task<double?> SumAsync(
            [NotNull] IQueryable<double?> source,
            CancellationToken cancellationToken = default);

        Task<double> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, double>> selector,
            CancellationToken cancellationToken = default);

        Task<double?> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, double?>> selector,
            CancellationToken cancellationToken = default);

        Task<float> SumAsync(
            [NotNull] IQueryable<float> source,
            CancellationToken cancellationToken = default);

        Task<float?> SumAsync(
            [NotNull] IQueryable<float?> source,
            CancellationToken cancellationToken = default);

        Task<float> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, float>> selector,
            CancellationToken cancellationToken = default);

        Task<float?> SumAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, float?>> selector,
            CancellationToken cancellationToken = default);

        Task<decimal> SumAsync(
            [NotNull] IEnumerable<decimal> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<decimal?> SumAsync(
            [NotNull] IEnumerable<decimal?> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<decimal> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, decimal>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<decimal?> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, decimal?>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<int> SumAsync(
            [NotNull] IEnumerable<int> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<int?> SumAsync(
            [NotNull] IEnumerable<int?> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<int> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, int>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<int?> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, int?>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<long> SumAsync(
            [NotNull] IEnumerable<long> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<long?> SumAsync(
            [NotNull] IEnumerable<long?> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<long> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, long>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<long?> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, long?>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double> SumAsync(
            [NotNull] IEnumerable<double> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double?> SumAsync(
            [NotNull] IEnumerable<double?> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, double>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double?> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, double?>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<float> SumAsync(
            [NotNull] IEnumerable<float> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<float?> SumAsync(
            [NotNull] IEnumerable<float?> enumerable,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), cancellationToken);

        Task<float> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, float>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<float?> SumAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, float?>> selector,
            CancellationToken cancellationToken = default) => SumAsync(enumerable.AsQueryable(), selector, cancellationToken);

        #endregion

        #region Average

        Task<decimal> AverageAsync(
            [NotNull] IQueryable<decimal> source,
            CancellationToken cancellationToken = default);

        Task<decimal?> AverageAsync(
            [NotNull] IQueryable<decimal?> source,
            CancellationToken cancellationToken = default);

        Task<decimal> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, decimal>> selector,
            CancellationToken cancellationToken = default);

        Task<decimal?> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, decimal?>> selector,
            CancellationToken cancellationToken = default);

        Task<double> AverageAsync(
            [NotNull] IQueryable<int> source,
            CancellationToken cancellationToken = default);

        Task<double?> AverageAsync(
            [NotNull] IQueryable<int?> source,
            CancellationToken cancellationToken = default);

        Task<double> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, int>> selector,
            CancellationToken cancellationToken = default);

        Task<double?> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, int?>> selector,
            CancellationToken cancellationToken = default);

        Task<double> AverageAsync(
            [NotNull] IQueryable<long> source,
            CancellationToken cancellationToken = default);

        Task<double?> AverageAsync(
            [NotNull] IQueryable<long?> source,
            CancellationToken cancellationToken = default);

        Task<double> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, long>> selector,
            CancellationToken cancellationToken = default);

        Task<double?> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, long?>> selector,
            CancellationToken cancellationToken = default);

        Task<double> AverageAsync(
            [NotNull] IQueryable<double> source,
            CancellationToken cancellationToken = default);

        Task<double?> AverageAsync(
            [NotNull] IQueryable<double?> source,
            CancellationToken cancellationToken = default);

        Task<double> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, double>> selector,
            CancellationToken cancellationToken = default);

        Task<double?> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, double?>> selector,
            CancellationToken cancellationToken = default);

        Task<float> AverageAsync(
            [NotNull] IQueryable<float> source,
            CancellationToken cancellationToken = default);

        Task<float?> AverageAsync(
            [NotNull] IQueryable<float?> source,
            CancellationToken cancellationToken = default);

        Task<float> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, float>> selector,
            CancellationToken cancellationToken = default);

        Task<float?> AverageAsync<T>(
            [NotNull] IQueryable<T> queryable,
            [NotNull] Expression<Func<T, float?>> selector,
            CancellationToken cancellationToken = default);

        Task<decimal> AverageAsync(
            [NotNull] IEnumerable<decimal> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<decimal?> AverageAsync(
            [NotNull] IEnumerable<decimal?> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<decimal> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, decimal>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<decimal?> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, decimal?>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double> AverageAsync(
            [NotNull] IEnumerable<int> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double?> AverageAsync(
            [NotNull] IEnumerable<int?> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, int>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double?> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, int?>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double> AverageAsync(
            [NotNull] IEnumerable<long> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double?> AverageAsync(
            [NotNull] IEnumerable<long?> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, long>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double?> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, long?>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double> AverageAsync(
            [NotNull] IEnumerable<double> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double?> AverageAsync(
            [NotNull] IEnumerable<double?> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<double> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, double>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<double?> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, double?>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<float> AverageAsync(
            [NotNull] IEnumerable<float> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<float?> AverageAsync(
            [NotNull] IEnumerable<float?> enumerable,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), cancellationToken);

        Task<float> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, float>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        Task<float?> AverageAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            [NotNull] Expression<Func<T, float?>> selector,
            CancellationToken cancellationToken = default) => AverageAsync(enumerable.AsQueryable(), selector, cancellationToken);

        #endregion

        #region ToList/Array

        Task<List<T>> ToListAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);


        Task<T[]> ToArrayAsync<T>(
            [NotNull] IQueryable<T> queryable,
            CancellationToken cancellationToken = default);

        Task<List<T>> ToListAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => ToListAsync(enumerable.AsQueryable(), cancellationToken);


        Task<T[]> ToArrayAsync<T>(
            [NotNull] IEnumerable<T> enumerable,
            CancellationToken cancellationToken = default) => ToArrayAsync(enumerable.AsQueryable(), cancellationToken);
        #endregion
    }
}
