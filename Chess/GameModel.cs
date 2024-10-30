using LavenderChessClock1.Validators;

namespace LavenderChessClock1.Chess
{
    public class GameModel
    {
        [SecondsValidator]
        public int Player1Seconds { get; set; } = 600;

        public IncrementType Player1Increment { get; set; } = IncrementType.NoIncrement;

        [IncrementLengthValidator]
        public int Player1IncrementLength { get; set; } = 0;

        public bool SameForBoth { get; set; } = true;

        [SecondsValidator]
        public int Player2Seconds { get; set; } = 600;

        public IncrementType Player2Increment { get; set; } = IncrementType.NoIncrement;

        [IncrementLengthValidator]
        public int Player2IncrementLength { get; set; } = 0;
    }
}
