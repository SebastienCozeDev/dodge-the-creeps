using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    // Run it when game over is detected.
    public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

		GetNode<Hud>("HUD").ShowGameOver();

		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    // Start a new game.
    public void NewGame()
	{
        _score = 0;

        var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

		var hud = GetNode<Hud>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

		GetNode<AudioStreamPlayer>("Music").Play();
    }

	// Call when the mob timer timeout.
	private void OnMobTimerTimeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		mob.Position = mobSpawnLocation.Position;

        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		var velocity = new Vector2((float)GD.RandRange(150f, 250f), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);
    }

    // Call when the score timer timeout.
    private void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<Hud>("HUD").UpdateScore(_score);
    }

    // Call when the start timer timeout.
    private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
}
