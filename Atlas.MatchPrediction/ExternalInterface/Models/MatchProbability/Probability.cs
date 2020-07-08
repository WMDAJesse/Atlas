using System;

namespace Atlas.MatchPrediction.ExternalInterface.Models.MatchProbability
{
    public class Probability
    {
        public static Probability Zero() => new Probability(0m);
        public static Probability One() => new Probability(1m);

        public Probability(decimal value)
        {
            Decimal = value;
        }

        public decimal Decimal { get; }

        #region Equality members

        protected bool Equals(Probability other)
        {
            return Decimal == other.Decimal;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Probability) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Decimal.GetHashCode();
        }

        public static bool operator ==(Probability left, Probability right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Probability left, Probability right)
        {
            return !Equals(left, right);
        }

        #endregion

        public int Percentage => Convert.ToInt32(Decimal * 100);

    }
}