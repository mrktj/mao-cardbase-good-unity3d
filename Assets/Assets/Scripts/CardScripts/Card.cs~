using UnityEngine;
using System;

public enum Suit {
  Hearts, Clubs, Diamonds, Spades
};

public enum Rank {
  Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
};

[Serializable]
public class Card {
  private const int minRank = 1;
  private const int maxRank = 13;

  private Rank _rank;
  private Suit _suit;

  public Rank rank { get { return _rank; }}
  public Suit suit { get { return _suit; }}
  public int cardValue { get { return ((int) rank - 1) + ((int) suit * maxRank); } }


  public Card (int cardValue) {
    _rank = (Rank) (cardValue % maxRank + 1);
    _suit = (Suit) (cardValue / maxRank);
  }

  public Card (Rank rank, Suit suit) {
    this._rank = rank;
    this._suit = suit;
  }
  
  public Card (int rank, Suit suit) {
    System.Diagnostics.Debug.Assert(minRank <= rank && rank <= maxRank);
    this._rank = (Rank) rank;
    this._suit = suit;
  }


  public override string ToString() {
    return RichText4();
  }

  public string NoRichText() {
    string output;
    if (rank == Rank.Ace)        output = "A";
    else if (rank == Rank.Jack)  output = "J";
    else if (rank == Rank.Queen) output = "Q";
    else if (rank == Rank.King)  output = "K";
    else                         output = ((int) rank).ToString();

    if (suit == Suit.Hearts)        output += "\u2665";
    else if (suit == Suit.Clubs)    output += "\u2663";
    else if (suit == Suit.Diamonds) output += "\u2666";
    else if (suit == Suit.Spades)   output += "\u2660";

    return output;
  }

  public string RichText2() {
    string output;
    if (rank == Rank.Ace)        output = "A";
    else if (rank == Rank.Jack)  output = "J";
    else if (rank == Rank.Queen) output = "Q";
    else if (rank == Rank.King)  output = "K";
    else                         output = ((int) rank).ToString();

    if (suit == Suit.Hearts)        output += "<color=red>\u2665</color>";
    else if (suit == Suit.Clubs)    output += "<color=black>\u2663</color>";
    else if (suit == Suit.Diamonds) output += "<color=red>\u2666</color>";
    else if (suit == Suit.Spades)   output += "<color=black>\u2660</color>";

    return output;
  }

  public string RichText4() {
    string output;
    if (rank == Rank.Ace)        output = "A";
    else if (rank == Rank.Jack)  output = "J";
    else if (rank == Rank.Queen) output = "Q";
    else if (rank == Rank.King)  output = "K";
    else                         output = ((int) rank).ToString();

    if (suit == Suit.Hearts)        output += "<color=red>\u2665</color>";
    else if (suit == Suit.Clubs)    output += "<color=green>\u2663</color>";
    else if (suit == Suit.Diamonds) output += "<color=blue>\u2666</color>";
    else if (suit == Suit.Spades)   output += "<color=black>\u2660</color>";

    return output;
  }
}
