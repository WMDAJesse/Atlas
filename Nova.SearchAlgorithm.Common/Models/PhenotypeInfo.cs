﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.SearchAlgorithm.Common.Models
{
    /// <summary>
    /// Data type to hold one instance of T for each of the five HLA loci and each type position within.
    /// </summary>
    /// <typeparam name="T">The type of the information that is required for each loci position.</typeparam>
    public class PhenotypeInfo<T>
    {
        public T A_1 { get; set; }
        public T A_2 { get; set; }
        public T B_1 { get; set; }
        public T B_2 { get; set; }
        public T C_1 { get; set; }
        public T C_2 { get; set; }
        public T DPB1_1 { get; set; }
        public T DPB1_2 { get; set; }
        public T DQB1_1 { get; set; }
        public T DQB1_2 { get; set; }
        public T DRB1_1 { get; set; }
        public T DRB1_2 { get; set; }

        // TODO: NOVA-1427: Mapping all positions in parallel using PLINQ may improve performance for long mapping functions
        public PhenotypeInfo<R> Map<R>(Func<Locus, TypePositions, T, R> mapping)
        {
            return new PhenotypeInfo<R>
            {
                A_1 = mapping(Locus.A, TypePositions.One, A_1),
                A_2 = mapping(Locus.A, TypePositions.Two, A_2),
                B_1 = mapping(Locus.B, TypePositions.One, B_1),
                B_2 = mapping(Locus.B, TypePositions.Two, B_2),
                C_1 = mapping(Locus.C, TypePositions.One, C_1),
                C_2 = mapping(Locus.C, TypePositions.Two, C_2),
                DPB1_1 = mapping(Locus.Dpb1, TypePositions.One, DPB1_1),
                DPB1_2 = mapping(Locus.Dpb1, TypePositions.Two, DPB1_2),
                DQB1_1 = mapping(Locus.Dqb1, TypePositions.One, DQB1_1),
                DQB1_2 = mapping(Locus.Dqb1, TypePositions.Two, DQB1_2),
                DRB1_1 = mapping(Locus.Drb1, TypePositions.One, DRB1_1),
                DRB1_2 = mapping(Locus.Drb1, TypePositions.Two, DRB1_2),
            };
        }
        
        // TODO: NOVA-1427: Mapping all positions in parallel using PLINQ may improve performance for long mapping functions
        public PhenotypeInfo<R> MapByLocus<R>(Func<Locus, T, T, Tuple<R, R>> mapping)
        {
            var a = mapping(Locus.A, A_1, A_2);
            var b = mapping(Locus.B, B_1, B_2);
            var c = mapping(Locus.C, C_1, C_2);
            var dpb1 = mapping(Locus.Dpb1, DPB1_1, DPB1_2);
            var dqb1 = mapping(Locus.Dqb1, DQB1_1, DQB1_2);
            var drb1 = mapping(Locus.Drb1, DRB1_1, DRB1_2);
            
            return new PhenotypeInfo<R>
            {
                A_1 = a.Item1,
                A_2 = a.Item2,
                B_1 = b.Item1,
                B_2 = b.Item2,
                C_1 = c.Item1,
                C_2 = c.Item2,
                DPB1_1 = dpb1.Item1,
                DPB1_2 = dpb1.Item2,
                DQB1_1 = dqb1.Item1,
                DQB1_2 = dqb1.Item2,
                DRB1_1 = drb1.Item1,
                DRB1_2 = drb1.Item2,
            };
        }

        // TODO: NOVA-1427: Running these tasks in parallel could likely improve performance
        public async Task<PhenotypeInfo<R>> MapAsync<R>(Func<Locus, TypePositions, T, Task<R>> mapping)
        {
            return new PhenotypeInfo<R>
            {
                A_1 = await mapping(Locus.A, TypePositions.One, A_1),
                A_2 = await mapping(Locus.A, TypePositions.Two, A_2),
                B_1 = await mapping(Locus.B, TypePositions.One, B_1),
                B_2 = await mapping(Locus.B, TypePositions.Two, B_2),
                C_1 = await mapping(Locus.C, TypePositions.One, C_1),
                C_2 = await mapping(Locus.C, TypePositions.Two, C_2),
                DPB1_1 = await mapping(Locus.Dpb1, TypePositions.One, DPB1_1),
                DPB1_2 = await mapping(Locus.Dpb1, TypePositions.Two, DPB1_2),
                DQB1_1 = await mapping(Locus.Dqb1, TypePositions.One, DQB1_1),
                DQB1_2 = await mapping(Locus.Dqb1, TypePositions.Two, DQB1_2),
                DRB1_1 = await mapping(Locus.Drb1, TypePositions.One, DRB1_1),
                DRB1_2 = await mapping(Locus.Drb1, TypePositions.Two, DRB1_2),
            };
        }

        // Aggregates each locus alongside its two values
        public IEnumerable<R> FlatMap<R>(Func<Locus, T, T, R> mapping)
        {
            return new List<R>
            {
                mapping(Locus.A, A_1, A_2),
                mapping(Locus.B, B_1, B_2),
                mapping(Locus.C, C_1, C_2),
                mapping(Locus.Dpb1, DPB1_1, DPB1_2),
                mapping(Locus.Dqb1, DQB1_1, DQB1_2),
                mapping(Locus.Drb1, DRB1_1, DRB1_2),
            };
        }

        public IEnumerable<T> ToEnumerable()
        {
            return new List<T> {A_1, A_2, B_1, B_2, C_1, C_2, DPB1_1, DPB1_2, DRB1_1, DRB1_2, DQB1_1, DQB1_2};
        }

        public void EachPosition(Action<Locus, TypePositions, T> action)
        {
            action(Locus.A, TypePositions.One, A_1);
            action(Locus.A, TypePositions.Two, A_2);
            action(Locus.B, TypePositions.One, B_1);
            action(Locus.B, TypePositions.Two, B_2);
            action(Locus.C, TypePositions.One, C_1);
            action(Locus.C, TypePositions.Two, C_2);
            action(Locus.Dpb1, TypePositions.One, DPB1_1);
            action(Locus.Dpb1, TypePositions.Two, DPB1_2);
            action(Locus.Dqb1, TypePositions.One, DQB1_1);
            action(Locus.Dqb1, TypePositions.Two, DQB1_2);
            action(Locus.Drb1, TypePositions.One, DRB1_1);
            action(Locus.Drb1, TypePositions.Two, DRB1_2);
        }

        public void EachLocus(Action<Locus, T, T> action)
        {
            action(Locus.A, A_1, A_2);
            action(Locus.B, B_1, B_2);
            action(Locus.C, C_1, C_2);
            action(Locus.Dpb1, DPB1_1, DPB1_2);
            action(Locus.Dqb1, DQB1_1, DQB1_2);
            action(Locus.Drb1, DRB1_1, DRB1_2);
        }

        public async Task WhenAllLoci(Func<Locus, T, T, Task> action)
        {
            await Task.WhenAll(
                action(Locus.A, A_1, A_2),
                action(Locus.B, B_1, B_2),
                action(Locus.C, C_1, C_2),
                action(Locus.Dpb1, DPB1_1, DPB1_2),
                action(Locus.Dqb1, DQB1_1, DQB1_2),
                action(Locus.Drb1, DRB1_1, DRB1_2));
        }

        public async Task<PhenotypeInfo<R>> WhenAllPositions<R>(Func<Locus, TypePositions, T, Task<R>> action)
        {
            R[] results = await Task.WhenAll(
                action(Locus.A, TypePositions.One, A_1),
                action(Locus.A, TypePositions.Two, A_2),
                action(Locus.B, TypePositions.One, B_1),
                action(Locus.B, TypePositions.Two, B_2),
                action(Locus.C, TypePositions.One, C_1),
                action(Locus.C, TypePositions.Two, C_2),
                action(Locus.Dpb1, TypePositions.One, DPB1_1),
                action(Locus.Dpb1, TypePositions.Two, DPB1_2),
                action(Locus.Dqb1, TypePositions.One, DQB1_1),
                action(Locus.Dqb1, TypePositions.Two, DQB1_2),
                action(Locus.Drb1, TypePositions.One, DRB1_1),
                action(Locus.Drb1, TypePositions.Two, DRB1_2));

            return new PhenotypeInfo<R>
            {
                A_1 = results[0],
                A_2 = results[1],
                B_1 = results[2],
                B_2 = results[3],
                C_1 = results[4],
                C_2 = results[5],
                DPB1_1 = results[6],
                DPB1_2 = results[7],
                DQB1_1 = results[8],
                DQB1_2 = results[9],
                DRB1_1 = results[10],
                DRB1_2 = results[11]
            };
        }

        public Tuple<T, T> DataAtLocus(Locus locus)
        {
            switch (locus)
            {
                case Locus.A:
                    return new Tuple<T, T>(A_1, A_2);
                case Locus.B:
                    return new Tuple<T, T>(B_1, B_2);
                case Locus.C:
                    return new Tuple<T, T>(C_1, C_2);
                case Locus.Dpb1:
                    return new Tuple<T, T>(DPB1_1, DPB1_2);
                case Locus.Dqb1:
                    return new Tuple<T, T>(DQB1_1, DQB1_2);
                case Locus.Drb1:
                    return new Tuple<T, T>(DRB1_1, DRB1_2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(locus), locus, null);
            }
        }

        public T DataAtPosition(Locus locus, TypePositions position)
        {
            const string errorMessage = "Can only fetch a single piece of data at a specific position";
            switch (locus)
            {
                case Locus.A:
                    switch (position)
                    {
                        case TypePositions.One:
                            return A_1;
                        case TypePositions.Two:
                            return A_2;
                        case TypePositions.Both:
                        case TypePositions.None:
                        default:
                            throw new Exception(errorMessage);
                    }
                case Locus.B:
                    switch (position)
                    {
                        case TypePositions.One:
                            return B_1;
                        case TypePositions.Two:
                            return B_2;
                        case TypePositions.Both:
                        case TypePositions.None:
                        default:
                            throw new Exception(errorMessage);
                    }
                case Locus.C:
                    switch (position)
                    {
                        case TypePositions.One:
                            return C_1;
                        case TypePositions.Two:
                            return C_2;
                        case TypePositions.Both:
                        case TypePositions.None:
                        default:
                            throw new Exception(errorMessage);
                    }
                case Locus.Dpb1:
                    switch (position)
                    {
                        case TypePositions.One:
                            return DPB1_1;
                        case TypePositions.Two:
                            return DPB1_2;
                        case TypePositions.Both:
                        case TypePositions.None:
                        default:
                            throw new Exception(errorMessage);
                    }
                case Locus.Dqb1:
                    switch (position)
                    {
                        case TypePositions.One:
                            return DQB1_1;
                        case TypePositions.Two:
                            return DQB1_2;
                        case TypePositions.Both:
                        case TypePositions.None:
                        default:
                            throw new Exception(errorMessage);
                    }
                case Locus.Drb1:
                    switch (position)
                    {
                        case TypePositions.One:
                            return DRB1_1;
                        case TypePositions.Two:
                            return DRB1_2;
                        case TypePositions.Both:
                        case TypePositions.None:
                        default:
                            throw new Exception(errorMessage);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(locus), locus, null);
            }
        }

        public void SetAtLocus(Locus locus, TypePositions positions, T value)
        {
            switch (locus)
            {
                case Locus.A:
                    if (positions == TypePositions.One || positions == TypePositions.Both)
                    {
                        A_1 = value;
                    }

                    if (positions == TypePositions.Two || positions == TypePositions.Both)
                    {
                        A_2 = value;
                    }

                    break;
                case Locus.B:
                    if (positions == TypePositions.One || positions == TypePositions.Both)
                    {
                        B_1 = value;
                    }

                    if (positions == TypePositions.Two || positions == TypePositions.Both)
                    {
                        B_2 = value;
                    }

                    break;
                case Locus.C:
                    if (positions == TypePositions.One || positions == TypePositions.Both)
                    {
                        C_1 = value;
                    }

                    if (positions == TypePositions.Two || positions == TypePositions.Both)
                    {
                        C_2 = value;
                    }

                    break;
                case Locus.Dpb1:
                    if (positions == TypePositions.One || positions == TypePositions.Both)
                    {
                        DPB1_1 = value;
                    }

                    if (positions == TypePositions.Two || positions == TypePositions.Both)
                    {
                        DPB1_2 = value;
                    }

                    break;
                case Locus.Dqb1:
                    if (positions == TypePositions.One || positions == TypePositions.Both)
                    {
                        DQB1_1 = value;
                    }

                    if (positions == TypePositions.Two || positions == TypePositions.Both)
                    {
                        DQB1_2 = value;
                    }

                    break;
                case Locus.Drb1:
                    if (positions == TypePositions.One || positions == TypePositions.Both)
                    {
                        DRB1_1 = value;
                    }

                    if (positions == TypePositions.Two || positions == TypePositions.Both)
                    {
                        DRB1_2 = value;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(locus), locus, null);
            }
        }

        #region Equality operators
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PhenotypeInfo<T>) obj);
        }

        private bool Equals(PhenotypeInfo<T> other)
        {
            return EqualityComparer<T>.Default.Equals(A_1, other.A_1) && EqualityComparer<T>.Default.Equals(A_2, other.A_2) &&
                   EqualityComparer<T>.Default.Equals(B_1, other.B_1) && EqualityComparer<T>.Default.Equals(B_2, other.B_2) &&
                   EqualityComparer<T>.Default.Equals(C_1, other.C_1) && EqualityComparer<T>.Default.Equals(C_2, other.C_2) &&
                   EqualityComparer<T>.Default.Equals(DPB1_1, other.DPB1_1) && EqualityComparer<T>.Default.Equals(DPB1_2, other.DPB1_2) &&
                   EqualityComparer<T>.Default.Equals(DQB1_1, other.DQB1_1) && EqualityComparer<T>.Default.Equals(DQB1_2, other.DQB1_2) &&
                   EqualityComparer<T>.Default.Equals(DRB1_1, other.DRB1_1) && EqualityComparer<T>.Default.Equals(DRB1_2, other.DRB1_2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<T>.Default.GetHashCode(A_1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(A_2);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(B_1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(B_2);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(C_1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(C_2);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(DPB1_1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(DPB1_2);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(DQB1_1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(DQB1_2);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(DRB1_1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(DRB1_2);
                return hashCode;
            }
        }

        public static bool operator ==(PhenotypeInfo<T> left, PhenotypeInfo<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PhenotypeInfo<T> left, PhenotypeInfo<T> right)
        {
            return !Equals(left, right);
        }
        
        #endregion
    }
}