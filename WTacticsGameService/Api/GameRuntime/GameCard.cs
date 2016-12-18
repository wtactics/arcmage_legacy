using System;

namespace WTacticsGameService.Api.GameRuntime
{
    public class GameCard
    {
        public Guid Id { get; set; }

        public string Url { get; set; }

        public bool IsFaceDown { get; set; }

        public bool IsMarked { get; set; }

        public bool IsDraggable { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public int CounterA { get; set; }

        public int CounterB { get; set; }

        public GameCard()
        {
            Id = Guid.NewGuid();
        }
    }
}
