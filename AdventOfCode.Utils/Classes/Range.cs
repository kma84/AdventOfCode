namespace AdventOfCode.Utils.Classes
{
    public record Range(long Start, long End)
    {
        public Range(long singleSeed) : this(singleSeed, singleSeed) { }

        public Range? Intersect(Range other)
        {
            Range? newRange = null;

            // |-----|
            //     |-----|
            if (other.Start > this.Start && other.Start <= this.End && other.End > this.End)
                newRange = new Range(other.Start, this.End);
            //     |-----|
            // |-----|
            else if (this.Start > other.Start && this.Start <= other.End && this.End > other.End)
                newRange = new Range(this.Start, other.End);
            //   |--|
            // |------|
            else if (this.Start >= other.Start && this.End <= other.End)
                newRange = this;
            // |------|
            //   |--|
            else if (this.Start < other.Start && this.End > other.End)
                newRange = other;

            return newRange;
        }

        public List<Range> Except(Range other)
        {
            List<Range> result = new();

            if (other.Start > this.Start && other.Start <= this.End)
            {
                result.Add(new Range(this.Start, other.Start - 1));

                if (other.End < this.End)
                {
                    result.Add(new Range(other.End + 1, this.End));
                }
            }
            else if (other.End < this.End && other.End >= this.Start)
            {
                result.Add(new Range(other.End + 1, this.End));
            }

            return result;
        }

    }
}
