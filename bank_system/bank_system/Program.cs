namespace bank_system;

internal class Program
{
  static void Main(string[] args)
  {
    BankovniUcet bank1 = new BankovniUcet("67",new PostovniAdresa("U Martina", "67", "Praguee", "17000"), 5000);
    BankovniUcet bank2 = new BankovniUcet("67",new PostovniAdresa("U Martina", "67", "Praguee", "17000"), 7000);


    bank1.Vlozit(67);
    Console.WriteLine(bank2.Vybrat(1000000));
    bank2.Vybrat(67);

    Console.WriteLine(bank1);
    BankovniUcet.ZobrazPocetAktivnichUctu();
  }
}

struct PostovniAdresa {
  public string Ulice {get; set;}
  public string CisloPopisne {get; set;}
  public string Mesto {get; set;}
  public string PSC {get; set;}

  public PostovniAdresa(string ulice, string cisloPopisne, string mesto, string psc) {
    Ulice = ulice;
    CisloPopisne = ulice;
    Mesto = mesto;
    Mesto = mesto;
    PSC = psc;
  }
}

class BankovniUcet {
  static private int _celkovyPocetUctu;
  private PostovniAdresa _adresaMajitele;

  private float _urokovaSazba;

  public string CisloUctu { get; set; }
  public decimal Zustatek { get; private set; }

  public float UrokovaSazba {
    get { return _urokovaSazba; }
    set { 
      if (value < 0) _urokovaSazba = 0;
      else if (value > 15) _urokovaSazba = 15;
      else _urokovaSazba = value;
    }
  }

  public BankovniUcet(string cisloUctu, PostovniAdresa adresaMajitele, decimal pocatecniVklad) {
    _celkovyPocetUctu ++;

    CisloUctu = cisloUctu;
    _adresaMajitele = adresaMajitele;

    if (pocatecniVklad < 0) {
      Zustatek = 0;
    } else {
      Zustatek = pocatecniVklad;
    }
  }

  public BankovniUcet(string cisloUctu, PostovniAdresa adresaMajitele) : this(cisloUctu, adresaMajitele, 0) {}

  public void Vlozit(decimal castka) {
    if (castka >= 0) {
      Zustatek += castka;
    }
  }

  public bool Vybrat(decimal castka) {
    if (castka <= Zustatek) {
      Zustatek -= castka;
      return true;
    } else return false;
  }

  public static void ZobrazPocetAktivnichUctu() {
    Console.WriteLine(_celkovyPocetUctu);
  }

  public override string ToString() {
    return $"Účet {CisloUctu} | Město: {_adresaMajitele.Mesto} | Zůstatek: {Zustatek} Kč";
  }
}














