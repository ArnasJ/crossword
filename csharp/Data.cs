using System.Collections.Immutable;
using System.Text;
using ExhaustiveMatching;
using LanguageExt;
using static LanguageExt.Prelude;

[Closed(typeof(Vowel), typeof(Consonant))]
public abstract record Letter;

public record Vowel(Option<VowelEnum> value) : Letter {
    public override string ToString() => value.Match(value => value.ToString(), () => "_");
}
public record Consonant(Option<ConsonantEnum> value) : Letter {
    public override string ToString() => value.Match(value => value.ToString(), () => "-");
}

public enum Direction {TTB, LTR}

public record Word(Arr<Letter> letters);

public record Cell((int x, int y) coords, Letter letter);

public record WordData {
    public Arr<Cell> cells;

    public WordData(Direction direction, (int x, int y) startCoords, Word word) {
        cells = CreateCells(direction, startCoords, word);
    }

    /// <summary>
    /// Finds the words that match Vowels and Consonants pattern in a given array.
    /// Returns empty array if non found.
    /// </summary>
    /// <param name="words"></param>
    /// <returns></returns>
    public Arr<Word> FindMatchingWords(Arr<Word> words) =>
        words
            .Where(word => cells.Length == word.letters.Length && IsSameLetterPattern(word))
            .ToArr();

    private bool IsSameLetterPattern(Word word) =>
        cells
            .Zip(word.letters)
            .All(tpl => tpl.Item1.letter.GetType() == tpl.Item2.GetType());

    private Arr<Cell> CreateCells(Direction direction, (int x, int y) startCoords, Word word) =>
        word.letters.Map(
            (i, letter) => new Cell(
                (
                    startCoords.x + (direction == Direction.LTR ? i : 0),
                    startCoords.y + (direction == Direction.LTR ? 0 : i)
                ),
                letter
            )).ToArr();
}

public record CrosswordState(Arr<Word> words, Arr<WordData> data, ImmutableDictionary<(int, int), Letter> crossword) {
    public static CrosswordState Create(Arr<Word> words, Arr<WordData> data) =>
        new CrosswordState(words, data, new Dictionary<(int, int), Letter>().ToImmutableDictionary());

    private readonly bool isSolved = data.IsEmpty;

    public Either<string, Arr<CrosswordState>> TrySolve() {
        return rec(Arr.create(this));

        Either<string, Arr<CrosswordState>> rec(Arr<CrosswordState> states) {
            var solvedStates = states.Where(state => state.isSolved);

            if (states.IsEmpty) {
                return Left("There are no solutions");
            }
        
            if (states.Length == solvedStates.Length) {
                return states;
            }

            var withNextWordSolved =
                states
                    .Except(solvedStates)
                    .ToArr()
                    .Bind(state => state.SolveNextWord());

            return rec(withNextWordSolved.AddRange(solvedStates));
        }
    }

    /// <summary>
    /// Places the given word into <see cref="crossword"/>> and removes the word and wordData from current state.
    /// Returns None, if word cannot be placed.
    /// </summary>
    /// <param name="wordToPlace"></param>
    /// <param name="wordToPlaceData"></param>
    /// <returns></returns>
    private Option<CrosswordState> PlaceWordIntoCrossword(Word wordToPlace, WordData wordToPlaceData) {
        var wordWithCoords = wordToPlaceData.cells
            .Zip(wordToPlace.letters)
            .Map(tpl => new KeyValuePair<(int, int), Letter>(tpl.Item1.coords, tpl.Item2))
            .ToArr();
    
        var canPlaceWord =
            wordWithCoords.All(kv => !crossword.TryGetValue(kv.Key, out var letter) || letter == kv.Value);
    
        return canPlaceWord
            ? new CrosswordState(
                words.Remove(wordToPlace),
                data.Remove(wordToPlaceData),
                crossword.AddRange(wordWithCoords)
            )
            : Option<CrosswordState>.None;
    }

    /// <summary>
    /// Returns all possible solutions to next <see cref="WordData"/> in current state.
    /// Returns empty array if next word cannot be solved.
    /// </summary>
    /// <returns></returns>
    private Arr<CrosswordState> SolveNextWord() {
        var wordData = data.Head();

        return wordData.FindMatchingWords(words)
            .Map(word => PlaceWordIntoCrossword(word, wordData))
            .Bind(_ => _)
            .ToArr();
    }
    
    public string ToVisual() {
        var maxX = crossword.Keys.Max(tpl => tpl.Item1);
        var maxY = crossword.Keys.Max(tpl => tpl.Item2);

        var sb = new StringBuilder();

        for (var y = 0; y <= maxY; y++) {
            for (var x = 0; x <= maxX; x++) {
                if (crossword.TryGetValue((x, y), out var letter)) {
                    sb.Append($"[{letter}]");
                }
                else {
                    sb.Append("   ");
                }
            }

            sb.AppendLine();
        }

        var str = sb.ToString();
        return str;
    }
}

public enum VowelEnum { A, Ą, E, Ę, Ė, I, Į, Y, O, U, Ų, Ū }
public enum ConsonantEnum { B, C, Č, D, F, G, H, J, K, L, M, N, P, R, S, Š, T, V, Z, Ž }