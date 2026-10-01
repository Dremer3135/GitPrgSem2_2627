namespace NejakaUloha;

internal class Program
{
  static void Main(string[] args)
  {
    StateMachine stateM = new StateMachine();

    Console.WriteLine();

    for (Int32 i = 0; i < 20; i++) {
      stateM.Tick();
      Console.WriteLine();
      Console.WriteLine($"{i+1}. krok");
      stateM.PrintState();
    }

    Console.WriteLine();
  }
}

class Position {
  public Int32 x, y;

  public Position(Int32 x, Int32 y) {
    this.x = x;
    this.y = y;
  }

  public bool Equals(Position another_pos) {
    return another_pos.x == this.x && another_pos.y == this.y;
  }

  public void Add(Position another_pos) {
    this.x += another_pos.x;
    this.y += another_pos.y;
  }

  public Position Plus(Position another_pos) {
    return new Position(this.x + another_pos.x, this.y + another_pos.y);
  }
}

class Orientation {
  private Position[] vectors = {new Position(1, 0), new Position(0, -1), new Position(-1, 0), new Position(0, 1)};
  public Int32 orientation = 0;

  public Orientation(Int32 orientation) {
    this.orientation = orientation;
    ClampOrientation();
  }

  private void ClampOrientation() {
    this.orientation = (this.orientation % 4 + 4) % 4;
  }

  public Position ToVector() {
    return vectors[this.orientation];
  }

  public void Add(Int32 another_orientation) {
    this.orientation += another_orientation;
    ClampOrientation();
  }

  public Orientation Plus(Int32 another_orientation) {
    return new Orientation(this.orientation + another_orientation);
  }
}

class Map {
  public Int32 width;
  public Int32 height;
  private Int32[,] map;
  
  public Map(char[,] char_map) {
    this.width = char_map.GetLength(0);
    this.height = char_map.GetLength(1);
    this.map = new Int32[this.width, this.height];
    for (Int32 y = 0; y < this.height; y++) {
      for (Int32 x = 0; x < this.width; x++) {
        Int32 block_type = BlockTypeFromChar(char_map[x, y]);
        if (block_type != -1) {
          SetAtPos(new Position(x, y), block_type);
        }
      }
    }
  }

  public Map(Map other) {
    this.width = other.width;
    this.height = other.height;
    this.map = (Int32[,])other.map.Clone();
  }

  public Int32 GetAtPos(Position pos) {
    return this.map[pos.x, pos.y];
  }

  public void SetAtPos(Position pos, Int32 block_type) {
    this.map[pos.x, pos.y] = block_type;
  }

  static public Int32 BlockTypeFromChar(char c) {
    if(c == 'X') {
      return 1;
    } else if (c == '.') {
      return 0;
    } else {
      return -1;
    }
  }

  static public char CharFromBlockType(Int32 block_type) {
    if (block_type == 0) {
      return '.';
    } else if (block_type == 1) {
      return 'X';
    } else {
      return ' ';
    }
  }
  
  private bool IsInBounds(Position pos) {  // returns wheather the target coordinates are inside the maze
    if (pos.x < 0 || pos.x >= this.width) return false;
    if (pos.y < 0 || pos.y >= this.height) return false;
    return true;
  }

  public char[,] GetCharMap() {
    char[,] char_map = new char[this.width, this.height];

    for (Int32 y = 0; y < this.height; y++) {
      for (Int32 x = 0; x < this.width; x++) {
        char_map[x, y] = Map.CharFromBlockType(this.GetAtPos(new Position(x, y)));
      }
    }

    return char_map;
  }

  public Map Clone() => new Map(this);
}

class StateMachine {
  private List<Creature> creatures = [];
  private Map maze;

  public StateMachine() {  // inits the state machine and gets the initial input   
    Int32 width = Convert.ToInt32(Console.ReadLine());
    Int32 height = Convert.ToInt32(Console.ReadLine());

    char[,] char_maze = new char[width, height];

    for(Int32 y = 0; y < height; y++) {
      Int32 x = 0;
      foreach (char char_block in Console.ReadLine()) {
        char_maze[x, y] = char_block;
        if (Map.BlockTypeFromChar(char_block) == -1) {
          creatures.Add(new Creature(new Position(x, y), Creature.GetOrientationFromChar(char_block)));
        }

        x++;
      }
    }

    maze = new Map(char_maze);
  }

  public void Tick() {
    foreach (Creature c in this.creatures) {
      c.Tick(this.GetMazeWithCreaturesAsWalls());
    }
  }

  private Map GetMazeWithCreaturesAsWalls() {
    Map maze_modified = this.maze.Clone();
    foreach (Creature c in this.creatures) {
      maze_modified.SetAtPos(c.pos, 1);
    }

    return maze_modified;
  }

  public void PrintState() {
    char[,] char_map = this.maze.GetCharMap();

    foreach (Creature c in this.creatures) {
      char_map[c.pos.x, c.pos.y] = c.GetCharFromOrientation();
    }

    for (Int32 y = 0; y < this.maze.height; y++) {
      for (Int32 x = 0; x < this.maze.width; x++) {
        Console.Write(char_map[x, y]);
      }

      Console.WriteLine();
    }

  }
}

class Creature {
  public Orientation orientation;
  public Position pos;
  private Int32 last_movevment = 0;

  public Creature(Position pos, Orientation orientation) {
    this.pos = pos;
    this.orientation = orientation;
  }

  public void Tick(Map map) {
    if (last_movevment == -1) {
      this.pos.Add(this.orientation.ToVector());
      last_movevment = 0;

    } else if (map.GetAtPos(this.pos.Plus(this.orientation.Plus(-1).ToVector())) == 0) {  // if there is free space to the front right
      this.orientation.Add(-1);
      last_movevment = -1;

    } else if (map.GetAtPos(this.pos.Plus(this.orientation.ToVector())) == 0) {  // if there is free space in front
      this.pos.Add(this.orientation.ToVector());
      last_movevment = 0;

    } else {
      this.orientation.Add(1);
      last_movevment = 1;
    }
  }

  public static Orientation GetOrientationFromChar(char c) {
    if (c =='>') return new Orientation(0);
    else if (c == '^') return new Orientation(1);
    else if (c == '<') return new Orientation(2);
    else return new Orientation(3);
  }

  public char GetCharFromOrientation() {
    if (this.orientation.orientation == 0) return '>';
    if (this.orientation.orientation == 1) return '^';
    if (this.orientation.orientation == 2) return '<';
    if (this.orientation.orientation == 3) return 'v';
    else return 'o';  // for debugging
  }
}

