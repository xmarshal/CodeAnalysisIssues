using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace Annotations
{
    [DebuggerStepThrough]
    public static class Check
    {
        [ContractAbbreviator]
        [ContractArgumentValidator]
        [ContractAnnotation("value:null => halt")]
        [NotNull]
        public static T NotNull<T>([ValidatedNotNull][NoEnumeration][CanBeNull] T value, [InvokerParameterName][NotNull] string parameterName)
            where T : class
        {
            Contract.Requires(value != null);
            Contract.Requires(!string.IsNullOrEmpty(parameterName));

            if (value is null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            Contract.EndContractBlock();

            return value;
        }

        [ContractAbbreviator]
        [ContractArgumentValidator]
        [ContractAnnotation("value:null => halt")]
        [NotNull]
        public static IReadOnlyList<T> NotEmpty<T>([ValidatedNotNull] IReadOnlyList<T> value, [InvokerParameterName][NotNull] string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Collection argument {0} is empty.", parameterName));
            }

            Contract.EndContractBlock();

            return value;
        }

        [ContractAbbreviator]
        [ContractArgumentValidator]
        [ContractAnnotation("value:null => halt")]
        [NotNull]
        public static TCollection NotEmpty<TCollection>([ValidatedNotNull] TCollection value, [InvokerParameterName][NotNull] string parameterName)
            where TCollection : class, ICollection
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Collection argument {0} is empty.", parameterName));
            }

            Contract.EndContractBlock();

            return value;
        }

        [ContractAbbreviator]
        [ContractArgumentValidator]
        [ContractAnnotation("value:null => halt")]
        [NotNull]
        public static string NotEmpty([ValidatedNotNull] string value, [InvokerParameterName][NotNull] string parameterName)
        {
            Exception e = null;

            if (value is null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} is empty.", parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            Contract.EndContractBlock();

            return value;
        }

        [CanBeNull]
        public static string NullButNotEmpty(string value, [InvokerParameterName][NotNull] string parameterName)
        {
            if (value is null)
            {
                return null;
            }

            if (value.Length == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument {0} is empty.", parameterName));
            }

            return value;
        }

        [ContractAbbreviator]
        [ContractArgumentValidator]
        [NotNull]
        public static IReadOnlyList<T> HasNoNulls<T>(IReadOnlyList<T> value, [InvokerParameterName][NotNull] string parameterName)
            where T : class
        {
            Contract.Requires(value != null);

            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            Contract.EndContractBlock();

            return value;
        }

        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        public static ref T InRange<T>(ref T value, string parameterName, T lowerBound, T upperBound)
            where T : struct, IComparable<T>
        {
            if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                var message = string.Format(CultureInfo.InvariantCulture, "Argument {0} must be in range [{1}, {2}].", parameterName, lowerBound, upperBound);
                throw new ArgumentOutOfRangeException(parameterName, value, message);
            }

            return ref value;
        }

        /// <summary>
        ///     Requires the range [offset, offset + count] to be a subset of [0, array.Count].
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> are out of range.</exception>
        public static void ArrayRange<T>(IList<T> array, int offset, int count, string offsetName, string countName)
        {
            Contract.Assert(!string.IsNullOrEmpty(offsetName));
            Contract.Assert(!string.IsNullOrEmpty(countName));
            Contract.Assert(array != null);

            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(countName);
            }

            if (offset < 0 || array.Count - offset < count)
            {
                throw new ArgumentOutOfRangeException(offsetName);
            }
        }
    }
}