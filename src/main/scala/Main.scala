import scala.io.Source

object Main {
  def main(args: Array[String]): Unit = {
    val file = Source.fromFile("input.txt").getLines.toVector
    val givenWords = file.head.split(" ").toVector
    val unparsedLines = file.tail.filter(_.nonEmpty)
    val parsedLines = unparsedLines.map(line =>
      line.zipWithIndex.map(c => Cell(c._2, unparsedLines.indexOf(line), tryGetLetter(c._1)))
    )
    val crossword = parsedLines.flatten
    val wordsToSolve = findWords(crossword)

    val possibleSolutions = solve(crossword, wordsToSolve, givenWords)

    possibleSolutions match {
      case None => println("No possible solutions.")
      case Some(solutions) => solutions.foreach(s => ???)
    }
  }

  sealed trait Letter
  case class Vowel(value: Option[Char]) extends Letter
  case class Consonant(value: Option[Char]) extends Letter

  case class Cell(xCoord: Int, yCoord: Int, letter: Option[Letter])

  case class Word(cells: Vector[Cell])

  def tryGetLetter(c: Char) : Option[Letter] =
    c match {
      case 'X' => Some(Consonant(None))
      case 'O' => Some(Vowel(None))
      case _ => None
    }

  def findWords(crossword: Vector[Cell]): Vector[Word] = {
    def go(crw: Vector[Cell], res: Vector[Word]): Vector[Word] = ???

    go(crossword, Vector.empty)
  }

  def solve(
    crossword: Vector[Cell],
    wordsToSolve: Vector[Word],
    givenWords: Vector[String]
  ) : Option[Vector[Vector[Cell]]] = ???
}
