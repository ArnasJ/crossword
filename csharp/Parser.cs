using LanguageExt;
using static LanguageExt.Prelude;

public static class Parser {
    public static Either<string, Arr<WordData>> ParseAllWordData(IEnumerable<string> lines) =>
        lines
            .Select(ParseWordData)
            .ToArr()
            .Sequence();

    private static Either<string, WordData> ParseWordData(string line) {
        var split = line.Split();
    
        if (split.Length != 4) {
            return Left("Incorrect amount of parameters.");
        }

        return
            from direction in ParseDirection(split[0])
            from coords in ParseCoordinates(split[1], split[2])
            from letters in ParseLetterNotation(split[3])
            select new WordData(direction, coords, new Word(letters));
    }

    private static Either<string, Direction> ParseDirection(string str) =>
        str switch {
            "ttb" => Right(Direction.TTB),
            "ltr" => Right(Direction.LTR),
            _ => Left($"Invalid direction: {str}.")
        };

    private static Either<string, (int x, int y)> ParseCoordinates(string xString, string yString) =>
        parseInt(xString)
            .Bind(x => parseInt(yString).Map(y => (x, y)))
            .ToEither("Bad coordinate input");

    private static Either<string, Arr<Letter>> ParseLetterNotation(string input) =>
        input
            .Map(c => c switch {
                'O' => Either<string, Letter>.Right(new Vowel(Option<VowelEnum>.None)),
                'X' => Either<string, Letter>.Right(new Consonant(Option<ConsonantEnum>.None)),
                _ => Either<string, Letter>.Left($"Bad input: {c} cannot be parsed as Vowel or Consonant.")
            })
            .ToArr()
            .Sequence();

    public static Either<string, Arr<Word>> ParseWords(string input) =>
        input
            .Split()
            .Map(ParseAsWord)
            .ToArr()
            .Sequence();

    private static Either<string, Word> ParseAsWord(string input) =>
        input
            .Map(ParseAsLetter)
            .ToArr()
            .Sequence()
            .Map(letters => new Word(letters));

    private static Either<string, Letter> ParseAsLetter(char input) {
        if (Enum.TryParse(input.ToString(), out VowelEnum vowel)) {
            return Either<string, Letter>.Right(new Vowel(vowel));
        }

        if (Enum.TryParse(input.ToString(), out ConsonantEnum consonant)) {
            return Either<string, Letter>.Right(new Consonant(consonant));
        }

        return Left($"'{input}' is not a valid letter.");
    }
}