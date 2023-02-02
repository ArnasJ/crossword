using LanguageExt;
using static LanguageExt.Prelude;

Arr.create("Input1.txt", "Input2.txt")
    .Map(SolveCrossword)
    .Map(ResultToVisual)
    .Iter(Console.WriteLine);

Either<string, Arr<CrosswordState>> SolveCrossword(string path) =>
    from lines in ReadFile(path)
    let wordsRaw = lines.Head()
    let dataRaw = lines.Skip(2)
    from words in Parser.ParseWords(wordsRaw)
    from data in Parser.ParseAllWordData(dataRaw)
    let crossword = CrosswordState.Create(words, data)
    from solutions in crossword.TrySolve()
    select solutions;

string ResultToVisual(Either<string, Arr<CrosswordState>> result) =>
    result.Match(
        Right: answers => string.Join('\n', answers.Map(s => s.ToVisual())),
        Left: error => error
    );

static Either<string, string[]> ReadFile(string filePath) =>
    Try(() => File.ReadAllLines(filePath)).ToEither(e => e.Message);