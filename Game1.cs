using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RaceingGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D playerTexture;
    private Texture2D enemy1Texture;
    private Texture2D enemy2Texture;
    private Texture2D enemy3Texture;
    private Texture2D roadTexture;
    //private SpriteFont scoreFont;

    private Vector2 playerPosition;
    private List<(Vector2 Position, Texture2D Texture)> enemies; // Lista för fiender med position och textur
    private Random random;
    private int screenWidth, screenHeight;
    private bool isGameOver = false;
    private int score = 0;

    private KeyboardState keyboardState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        screenWidth = _graphics.PreferredBackBufferWidth;
        screenHeight = _graphics.PreferredBackBufferHeight;

        playerPosition = new Vector2(screenWidth / 2 - 25, screenHeight - 100);
        enemies = new List<(Vector2, Texture2D)>(); // Initiera lista för fiender
        random = new Random();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        playerTexture = Content.Load<Texture2D>("Player-car");
        enemy1Texture = Content.Load<Texture2D>("Enemy-car1");
        enemy2Texture = Content.Load<Texture2D>("Enemy-car2");
        enemy3Texture = Content.Load<Texture2D>("Enemy-car3");
        roadTexture = Content.Load<Texture2D>("Road");
        //scoreFont = Content.Load<SpriteFont>("ScoreFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (isGameOver) return;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Left) && playerPosition.X > 50)
        {
            playerPosition.X -= 5;
        }
        if (keyboardState.IsKeyDown(Keys.Right) && playerPosition.X < screenWidth - 50 - playerTexture.Width)
        {
            playerPosition.X += 5;
        }

        // Skapa nya fiender slumpmässigt
        if (random.Next(100) < 3)
        {
            float xPos = random.Next(50, screenWidth - 50 - enemy1Texture.Width);
            //float xPos = 150;
            Texture2D randomEnemyTexture = GetRandomEnemyTexture(); // Välj slumpmässig fiendetextur
            enemies.Add((new Vector2(xPos, -randomEnemyTexture.Height), randomEnemyTexture));
        }

        // Uppdatera fiendernas positioner och kolla kollisioner
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            Vector2 newPosition = new Vector2(enemy.Position.X, enemy.Position.Y + 5);
            enemies[i] = (newPosition, enemy.Texture);

            if (new Rectangle((int)playerPosition.X, (int)playerPosition.Y, playerTexture.Width, playerTexture.Height)
                .Intersects(new Rectangle((int)newPosition.X, (int)newPosition.Y, enemy.Texture.Width, enemy.Texture.Height)))
            {
                isGameOver = true;
            }
        }

        // Ta bort fiender som har lämnat skärmen
        enemies.RemoveAll(enemy => enemy.Position.Y > screenHeight);

        score++;
        base.Update(gameTime);
    }

    private Texture2D GetRandomEnemyTexture()
    {
        // Slumpmässigt välj en av de tre fiendetexturerna
        int index = random.Next(3);
        return index switch
        {
            0 => enemy1Texture,
            1 => enemy2Texture,
            2 => enemy3Texture,
            _ => enemy1Texture // Fallback
        };
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);


        _spriteBatch.Begin();
        int roadWidth = roadTexture.Width;
        int roadHeight = roadTexture.Height;
        
        // Scale the road texture to fit the screen while maintaining its aspect ratio
        float scale = (float)screenHeight / roadHeight;
        int scaledWidth = (int)(roadWidth * scale);
        
        _spriteBatch.Draw(
            roadTexture,
            new Rectangle((screenWidth - scaledWidth) / 2, 0, scaledWidth, screenHeight),
            Color.White
        );
        //_spriteBatch.Draw(roadTexture, new Rectangle(50, 0, screenWidth - 100, screenHeight), Color.White);

/*
        if (isGameOver)
        {
            _spriteBatch.DrawString(scoreFont, "Game Over! Score: " + score, new Vector2(screenWidth / 2 - 100, screenHeight / 2), Color.Red);
        }
        else
        {
            _spriteBatch.Draw(playerTexture, playerPosition, Color.White);

            // Rita alla fiender
            foreach (var enemy in enemies)
            {
                _spriteBatch.Draw(enemy.Texture, enemy.Position, Color.White);
            }

            // Rita poängen
            _spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(10, 10), Color.White);
        }
*/
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}