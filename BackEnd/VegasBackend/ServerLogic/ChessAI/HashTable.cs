namespace VegasBackend.ServerLogic.ChessAI
{
    public enum TTEntryType
    {
        Exact,      // Exact score
        LowerBound, // Alpha cutoff (score >= beta)
        UpperBound  // Beta cutoff (score <= alpha)
    }

    public class TTEntry
    {
        public ulong Hash { get; set; }
        public int Depth { get; set; }
        public int Score { get; set; }
        public TTEntryType Type { get; set; }
        public string BestMove { get; set; } // e.g., "e2e4"

        public TTEntry(ulong hash, int depth, int score, TTEntryType type, string bestMove = null)
        {
            Hash = hash;
            Depth = depth;
            Score = score;
            Type = type;
            BestMove = bestMove;
        }
    }

    public class TranspositionTable
    {
        private readonly Dictionary<ulong, TTEntry> _table;
        private readonly int _maxSize;
        public int Hits { get; private set; }
        public int Collisions { get; private set; }

        public TranspositionTable(int maxSizeMB = 64)
        {
            // Approximate: each entry ~40 bytes, so 1MB ≈ 25,000 entries
            _maxSize = maxSizeMB * 25000;
            _table = new Dictionary<ulong, TTEntry>(_maxSize);
            Hits = 0;
            Collisions = 0;
        }

        public void Store(ulong hash, int depth, int score, TTEntryType type, string bestMove = null)
        {
            // Always replace or don't replace based on depth
            if (_table.TryGetValue(hash, out var existing))
            {
                // Replace if deeper search or same depth
                if (depth >= existing.Depth)
                {
                    _table[hash] = new TTEntry(hash, depth, score, type, bestMove);
                }
                else
                {
                    Collisions++;
                }
            }
            else
            {
                // If table is full, remove random entry (simple replacement scheme)
                if (_table.Count >= _maxSize)
                {
                    var keyToRemove = _table.Keys.First();
                    _table.Remove(keyToRemove);
                }

                _table[hash] = new TTEntry(hash, depth, score, type, bestMove);
            }
        }

        public bool TryGetValue(ulong hash, int depth, int alpha, int beta, out int score, out string bestMove)
        {
            score = 0;
            bestMove = null;

            if (!_table.TryGetValue(hash, out var entry))
                return false;

            // Only use if searched at equal or greater depth
            if (entry.Depth < depth)
                return false;

            Hits++;
            bestMove = entry.BestMove;

            // Check if we can use this score
            switch (entry.Type)
            {
                case TTEntryType.Exact:
                    score = entry.Score;
                    return true;

                case TTEntryType.LowerBound:
                    if (entry.Score >= beta)
                    {
                        score = entry.Score;
                        return true;
                    }
                    break;

                case TTEntryType.UpperBound:
                    if (entry.Score <= alpha)
                    {
                        score = entry.Score;
                        return true;
                    }
                    break;
            }

            return false;
        }

        public void Clear()
        {
            _table.Clear();
            Hits = 0;
            Collisions = 0;
        }

        public string GetStats()
        {
            return $"TT: {_table.Count} entries, {Hits} hits, {Collisions} collisions";
        }
    }
}