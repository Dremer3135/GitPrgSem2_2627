namespace psm;

internal class Program {
  public static List<Person> men = new List<Person>();
  public static List<Person> women = new List<Person>();

  static void Main(string[] args) {
    Int32 length = Convert.ToInt32(Console.ReadLine());

    for (int i = 0; i < length; i++) {
      List<Int32> preferences = new List<int>();
      foreach (string preference_string in Console.ReadLine().Split(' ')) {
        preferences.Add(Convert.ToInt32(preference_string));
      }

      women.Add(new Person(preferences));
    }

    for (int i = 0; i < length; i++) {
      List<Int32> preferences = new List<int>();
      foreach (string preference_string in Console.ReadLine().Split(' ')) {
        preferences.Add(Convert.ToInt32(preference_string));
      }

      men.Add(new Person(preferences));
    }




    while (true) {
      // foreach (Person woman in women) woman.ClearProposals();
      foreach (Person man in men) man.ClearProposals();
      
      foreach (Person woman in women) {
        men[woman.preferences[0]].Propose(woman.my_id);
      }

      foreach (Person man in men) {
        Int32 proposal = man.PickFromProposals();

        if (proposal != 0) {
          women[proposal].Match_with(man.my_id);
        }
      }

      foreach (Person woman in women) {
        if (!woman.IsMatched()) {
          woman.ProposalDidntWorkOut();
        }
      }
    }







  }
}

public class Person {
  public Int32 my_id;
  public Int32 match = 0;
  public List<Int32> preferences;
  public List<Int32> current_proposals = new List<int>();

  public Person(List<Int32> preferences) {
    this.preferences = preferences;
  }

  public Int32 PickFromProposals() {
    if (current_proposals.Count() == 0) return 0;

    foreach (Int32 preference in this.preferences) {
      if (this.current_proposals.Contains(preference)) return preference;
    }

    Console.Write("Something went really wrong");
    return -67;
  }

  public void Propose(Int32 other) {
    this.current_proposals.Add(other);
  }

  public void Match_with(Int32 other) {
    this.match = other;
  }

  public bool IsMatched() {
    return match != 0;
  }

  public void ClearProposals() {
    this.current_proposals.Clear();
  }

  public void ProposalDidntWorkOut() {
    this.preferences.RemoveAt(0);
  }
}












