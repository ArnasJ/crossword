import scala.io.Source

object Main {
  def main(args: Array[String]): Unit = {
    val file = Source.fromFile("input.txt").getLines.toList
    val givenWords = file.head.split(" ")
    val crosswordLines = file.tail.filter(_.nonEmpty)
    val numberOfRows = crosswordLines.map(_.length).max
    val numberOfColumns = crosswordLines.length
  }

  sealed trait Letter
  case class Vowel(value: Option[Char]) extends Letter
  case class Consonant(value: Option[Char]) extends Letter

  case class Cell(xCoord: Int, yCoord: Int, isFirst: Boolean, letter: Letter)

  case class Word(cells: List[Cell])

  case class Crossword(words: List[Word])
}
