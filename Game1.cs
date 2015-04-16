using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Scarab_Commando
{
   
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Variáveis de som.Dois SoundBanks são usados para diferenciar música de efeitos sonoros       
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        SoundBank musicBank;

        // Texturas do fundo de tela e da tela inicial
        Texture2D background;
        Texture2D mainscreen;

        // Retângulo aonde o fundo será desenhado
        Rectangle backgroundRect;

        // Lista de pirâmides
        List<Sprite> pyramids;

        // Lista de alvos
        List<Target> targets;

        // Variáveis usadas para preparar o Hadouken
        float hadoukenTimer = 0f;
        int combo = 0;

        // Sprite estático de Goku
        Sprite goku;

        // Objeto animado Gundam
        Gundam gundam;

        // Variável para determinar dificuldade
        Difficulty difficulty;

        // Contador de vidas
        int counter;

        // Determina se o jogador perdeu
        bool lost=false;

        // Projéteis disparados pelo Gundam
        Projectile hadouken, scarab;

        // Lista os possíveis estados de jogo 
        enum GameState{Menu,Pause,Playing,GameOver, Options};

        // Lista as possíveis dificuldades, atribuindo valores numéricos
        enum Difficulty { Easy=1, Normal=2, Hard=3 ,Insane=9};

        // Variável que determina o estado atual do jogo
        GameState gameState;

        // Variável de fonte usada para escrever na tela
        SpriteFont font;

        // Placar
        int score;

        // Animação da explosão
        Animation explosion;

        // Escala do fundo de tela, permite redimensioná-lo sem alterar a imagem
        #region Scales

       
        protected float backgroundScale = 1.0f;
        public float BackgroundScale
        {
            get { return backgroundScale; }
            set
            {
                backgroundScale = value;

            }
        }

        #endregion
       

        
         
        #endregion
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Torna o mouse visível
            this.IsMouseVisible = true;
        }

        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
           // Inicializa as variáveis de som
            audioEngine = new AudioEngine("soundtrack.xgs");
            waveBank = new WaveBank(audioEngine,"Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine,"Sound Bank.xsb");
            musicBank = new SoundBank(audioEngine,"Sound Bank.xsb");

            // Inicializa as listas de pirâmides e alvos
            pyramids = new List<Sprite>();
            targets = new List<Target>();

            // Inicializa o Gundam
            gundam = new Gundam();

            // Inicializa o Goku
            goku = new Sprite();

            // Inicializa os projéteis, usando seu tipo como parâmetro
            hadouken = new Projectile("hadouken");
            scarab = new Projectile("scarab");

            // Inicializa o estado como Menu (tela inicial)
            gameState = GameState.Menu;

            // Inicializa com dificuldade normal
            difficulty = Difficulty.Normal;

            // Inicia o Gundam virado para a direita
            gundam.OrientationX = true;

            // Inicializa a explosão
            explosion = new Animation();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Carrega a fonte (Courier New)
            font = Content.Load<SpriteFont>("Courier New");

            // Carrega a tela inicial e o fundo de tela
            mainscreen = Content.Load<Texture2D>("mainscreen");
            background = Content.Load<Texture2D>("desert");
            
            // Carrega o retângulo 
           backgroundRect  = new Rectangle(0, 0, background.Width, background.Height);

            // Carrega todas as pirâmides com a mesma imagem
           for (int i = 0; i < 6; i++)
           {
              pyramids.Add(new Sprite());
               pyramids[i].LoadContent(Content, "pyramid");

           }
            // Determina a posição inicial da primeira pirâmide
           pyramids[0].Position = new Vector2(20, this.GraphicsDevice.Viewport.Height - pyramids[0].Size.Height);

            // Determina a posição inicial das pirâmides restantes baseando-se na primeira
           for (int i = 1; i < 6; i++)
           {
               pyramids[i].position.X = pyramids[i - 1].Position.X + pyramids[i].Size.Width;
               pyramids[i].position.Y = pyramids[i - 1].position.Y;
           }

            // Carrega a imagem do Goku
           goku.LoadContent(Content, "goku");

            // Determina a posição do Goku
           goku.Position = new Vector2((GraphicsDevice.Viewport.Width / 2) - (goku.size.Width / 2), (GraphicsDevice.Viewport.Height - goku.size.Height) / 2);

            // Carrega a sprite sheet do Gundam
           gundam.LoadContent(Content, "gundamsheet");

            // Determina a posição do Gundam
           gundam.Position = new Vector2((GraphicsDevice.Viewport.Width / 2) - (gundam.Width / 2), (GraphicsDevice.Viewport.Height/2 + gundam.Height/2) );

            // Guarda a posição inicial numa variável separada
           gundam.InitialPosition = gundam.Position;

            // Determina a altura máxima a que o Gundam pode chegar
           gundam.MaxY = gundam.InitialPosition.Y-200;

            // Atribui o tamanho de cada frame da animação do Gundam a variáveis de tamanho
           gundam.Width = 143;
           gundam.Height = 104;

            // Determina a duração de cada frame da animação do Gundam
           gundam.Interval = 60f;

            // Carrega o meteoro e o reseta
           targets.Add(new Target("meteor"));
              targets[0].LoadContent(Content, "meteor");
               targets[0].Reset(difficulty.ToString());

            // Carrega a Genki Dama e a reseta
               targets.Add(new Target("genkidama"));
               targets[1].LoadContent(Content, "genkidama");
               targets[1].Reset(difficulty.ToString());

            // Carrega os dois projéteis
               hadouken.LoadContent(Content, "hadouken");
               scarab.LoadContent(Content, "scarab");
           
            // Carrega a sprite sheet da explosão
               explosion.LoadContent(Content,"expmenor");
               explosion.Width = 50;
               explosion.Height = 50;
               explosion.FrameCount = 8;
           
            // Coloca o número de vidas igual ao número total de pirâmides
           counter = pyramids.Count;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Muda o estado de jogo dependendo da variável gameState
            switch(gameState)
            {
                #region Menu
                case GameState.Menu:
                    {
                        // Zera a vibração do controle enquanto estiver na tela inicial
                        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                        // Para sons que poderiam estar tocando
                        if (!soundBank.IsDisposed)
                            soundBank.Dispose();
                        if (!musicBank.IsDisposed)
                            musicBank.Dispose();
                       
                        // Começa o jogo baseado em input
                        if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)) 
                        {
                            ResetGame(); gameState = GameState.Playing;
                        }

                        // Vai para a tela de opções baseado em input
                if (Keyboard.GetState().IsKeyDown(Keys.O)||GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y))
                {
                    difficulty = Difficulty.Normal;
                    gameState = GameState.Options;
                }
                        break;
                    }
                #endregion
                #region Gameplay
                case GameState.Playing:
                    {
                        // Testa se o Gundam está fora da tela
                        gundam.TestOutOfSight(graphics.GraphicsDevice);

                        // Obtém um array com todas as teclas pressionadas
                        Keys[] theKeys = Keyboard.GetState().GetPressedKeys();

                        // Anima o Gundam
                        gundam.Animate(gameTime);

                        // Aumenta a dificuldade dependendo do número de pontos
                        DifficultyUp();
                        
                        
                        #region Controls
                       
                        // Deixa o Gundam parado se nenhuma ação estiver sendo executada e nenhuma tecla estiver sendo pressionada
                        if (((theKeys.Length == 0) && (gundam.state != Gundam.State.Attacking) && (gundam.state != Gundam.State.Falling) && (gundam.state != Gundam.State.Jumping) && (gundam.state != Gundam.State.JumpAttacking)))
                        { gundam.state = Gundam.State.Still; }

                        // O Gundam pula se o botão estiver pressionado e ele não estiver atacando, pulando ou pulando e atacando
                        if (((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))||(Keyboard.GetState().IsKeyDown(Keys.Space)))&&(gundam.state!=Gundam.State.Attacking)&&(gundam.state!=Gundam.State.Falling)&&(gundam.state!=Gundam.State.Jumping)&&(gundam.state!=Gundam.State.JumpAttacking))
                        {
                            gundam.CurrentFrame = 4;
                            gundam.state = Gundam.State.Jumping;
                        }


                        // Gundam ataca se o botão for pressionado e ele não estiver em nenhum estado que impediria o ataque
                        if (((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)||(Mouse.GetState().RightButton==ButtonState.Pressed)) && (gundam.state != Gundam.State.Attacking) && (gundam.state != Gundam.State.Jumping) && (gundam.state != Gundam.State.Falling) && (gundam.state != Gundam.State.JumpAttacking))
                        {
                            gundam.CurrentFrame = 9;
                            gundam.state = Gundam.State.Attacking;
                        }

                        // Muda o estado do Gundam para "andando" caso algum botão de movimento estiver sendo pressionado
                        if ((((Keyboard.GetState().IsKeyDown(Keys.A)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)) || (Keyboard.GetState().IsKeyDown(Keys.D)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight))) && (gundam.state != Gundam.State.Jumping) && (gundam.state != Gundam.State.Falling) && (gundam.state != Gundam.State.Attacking) && (gundam.state != Gundam.State.JumpAttacking)))
                        {
                            gundam.state = Gundam.State.Walking;
                        }

                        // Muda a orientação do Gundam para a esquerda
                        if (((Keyboard.GetState().IsKeyDown(Keys.A)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft))) && (gundam.Position.X > 15) && (gundam.state != Gundam.State.Attacking))
                        {
                            gundam.OrientationX = false;

                        }

                        // Muda a orientação do Gundam para a direita
                        if (((Keyboard.GetState().IsKeyDown(Keys.D)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight))) && (gundam.Position.X + gundam.Width < graphics.GraphicsDevice.Viewport.Width - 15) && (gundam.state != Gundam.State.Attacking))
                        {
                            gundam.OrientationX = true;
                       
                        }
                        
                        // Aumenta a velocidade dos meteoros. Usado preferencialmente para testes.
                        if ((Keyboard.GetState().IsKeyDown(Keys.D)&&(Keyboard.GetState().IsKeyDown(Keys.I))&&(Keyboard.GetState().IsKeyDown(Keys.E))))
                        {
                            targets[0].speed.Y = 3000;
                        }

                        // Retorna para a tela inicial
                        if (Keyboard.GetState().IsKeyDown(Keys.Escape)||(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)))
                        {
                            musicBank.Dispose();
                            soundBank.Dispose();
                            gameState=GameState.Menu;
                            score = 0;

                        }

                        // Inicia o disparo de um escaravelho baseado em input
                        if (((Mouse.GetState().LeftButton == ButtonState.Pressed) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X))) && (!scarab.Shooting))
                        { scarab.Shoot = true; }

                        // Pausa o jogo se 'P' for pressionado
                        if (Keyboard.GetState().IsKeyDown(Keys.P)||GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftTrigger))
                        {
                            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                            musicBank.Dispose();
                            soundBank.Dispose(); gameState = GameState.Pause;
                        }

                        #region Shoot

                        // Se o disparo de um escaravelho tiver sido iniciado, chama o método de disparo
                        if (scarab.Shoot)
                        {
                            // O método possui uma versão para Mouse e outra para GamePad; por isso as duas condições
                            if ((Mouse.GetState().LeftButton == ButtonState.Pressed) && (!scarab.Shooting))
                            {
                                Shoot(scarab, new Vector2(1500, 1500), "scarab");
                            }
                            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X)) && (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left != Vector2.Zero) && (!scarab.Shooting))
                            {
                                Shoot(scarab, new Vector2(1500, 1500), "scarab", GamePad.GetState(PlayerIndex.One).ThumbSticks.Left);
                            }
                        }

                        // Se o disparo de um Hadouken tiver sido iniciado, chama o método de disparo
                        if (hadouken.Shoot)
                        {
                            // O método possui uma versão para Mouse e outra para GamePad; por isso as duas condições
                            if ((Mouse.GetState().RightButton == ButtonState.Pressed) && (!hadouken.Shooting))
                            {
                                Shoot(hadouken, new Vector2(2000, 2000), "hadouken");
                            }
                            if (((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))) && (!hadouken.Shooting))
                            {
                                Shoot(hadouken, new Vector2(2000, 2000), "hadouken", GamePad.GetState(PlayerIndex.One).ThumbSticks.Left);
                            }
                        }
                        #endregion
                        #region Shooting
                        // Se o escaravelho estiver sendo disparado, chama o método que roda durante o disparo
                        if (scarab.Shooting)
                        {
                            Shooting(scarab, targets, gameTime);
                        }

                        // Se o Hadouken estiver sendo disparado, chama o método que roda durante o disparo
                        if (hadouken.Shooting)
                        {
                            Shooting(hadouken, targets, gameTime);
                        }

                        #endregion
                        #endregion
                        #region Sound
                        // Inicia os bancos de som caso estejam dispensados
                        if (musicBank.IsDisposed)
                        {
                            musicBank = new SoundBank(audioEngine, "Sound Bank.xsb");
                        } 
                        if (soundBank.IsDisposed)
                        {
                            soundBank = new SoundBank(audioEngine, "Sound Bank.xsb");
                        }

                        // Toca a música se o banco de música não estiver sendo usado
                        if (!musicBank.IsInUse)
                        {
                            musicBank.PlayCue("battle");
                        }
                        
                        #endregion
                        #region Explosion dealing

                        // Se a explosão for iniciada, toca o som correspondente e inicia o processo para a animação
                        if (explosion.IsOn)
                        {
                            soundBank.PlayCue("explosion");
                            explosion.IsGoing = true;
                            explosion.IsOn=false;
                        }

                        // Se a animação da explosão estiver sendo processada, chama o método de animação
                        if (explosion.IsGoing)
                        {
                            
                            explosion.Animate(gameTime, explosion.Position);

                        }
                        #endregion
                        #region Check loss
                        // Jogador perde se o contador de vidas for 0 ou menor
                        if (counter <= 0)
                        {
                            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                            lost = true;
                            soundBank.Dispose();
                            musicBank.Dispose();
                            gameState = GameState.GameOver;

                        }
                        #endregion


                        #region ComboHadouken
                        // Muda a sequência dependendo do sentido do Gundam
                        switch(gundam.OrientationX)
                        {
                            case true:
                                {
                                    // Se o primeiro botão for pressionado, passa para o próximo passo do combo e dá 1 segundo para que o jogador o realize
                                    if (((Keyboard.GetState().IsKeyDown(Keys.A)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown))) && (hadoukenTimer <= 0)&&((!(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight)))||(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))))
                                    {
                                        combo = 1;
                                        hadoukenTimer += 1f;
                                    }

                                    // Se o segundo botão for pressionado, passa para o próximo passo do combo e dá 1 segundo para que o jogador o realize
                                    if ((combo == 1) && (hadoukenTimer > 0) && ((Keyboard.GetState().IsKeyDown(Keys.S)) || ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown)) && (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))) || ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown)) && (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight)))))
                                    {
                                        combo = 2;
                                        hadoukenTimer += 1f;
                                    }

                                    // Se o terceiro botão for pressionado, passa para o próximo passo do combo
                                    if ((combo == 2) && (hadoukenTimer > 0) && ((Keyboard.GetState().IsKeyDown(Keys.D)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight))))
                                        combo = 3;
                                    break;
                                }
                            case false:
                                {
                                    // Se o primeiro botão for pressionado, passa para o próximo passo do combo e dá 1 segundo para que o jogador o realize
                                    if (((Keyboard.GetState().IsKeyDown(Keys.D)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown))) && (hadoukenTimer <= 0) && ((!(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft))) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))))
                                    {
                                        combo = 1;
                                        hadoukenTimer += 1f;
                                    }

                                    // Se o segundo botão for pressionado, passa para o próximo passo do combo e dá 1 segundo para que o jogador o realize
                                    if ((combo == 1) && (hadoukenTimer > 0) && ((Keyboard.GetState().IsKeyDown(Keys.S)) || ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown)) && (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))) || ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown)) && (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)))))
                                    {
                                        combo = 2;
                                        hadoukenTimer += 1f;
                                    }

                                    // Se o terceiro botão for pressionado, passa para o próximo passo do combo
                                    if ((combo == 2) && (hadoukenTimer > 0) && ((Keyboard.GetState().IsKeyDown(Keys.A)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft))))
                                        combo = 3;
                                    break;
                                
                                }
                    }
                        // Diminui o tempo restante para realizar o próximo passo da sequência
                            hadoukenTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Se o combo estiver no passo final e o botão for pressionado, inicia o hadouken, coloca um delay de 0.5s para usar o próximo e zera o combo
                        if ((combo == 3) && ((Mouse.GetState().RightButton == ButtonState.Pressed)||(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))) && (!hadouken.Shooting))
                        {
                            hadouken.Shoot = true;
                            hadoukenTimer = 0.5f;
                            combo = 0;
                        }
                        #endregion
                        
                        #region Collision Detection

                        // Atualiza cada pirâmide
                        foreach (Sprite spr in pyramids)
                        {
                            spr.Update(gameTime, Vector2.Zero);

                        }
                        // Reseta os alvos caso estejam fora da tela
                            if (targets[0].OutOfSight(graphics.GraphicsDevice))
                            {
                                targets[0].Reset(difficulty.ToString());
                            }
                            // Adiciona rotação aos alvos
                                targets[0].Rotation += 0.1f;

                        // Checa colisões em cada alvo
                                foreach (Target t in targets)
                                {
                                    // Checa colisão com espadada do Gundam 
                                    if ((t.Collision(gundam,gundam.Width,gundam.Height))&&((gundam.state==Gundam.State.Attacking)||(gundam.state==Gundam.State.JumpAttacking))&&(!(gundam.Hit)))
                                    {
                                        // Coloca a posição da explosão como o centro do alvo
                                        explosion.Position = new Vector2(t.Position.X + t.size.Width / 2, t.Position.Y + t.size.Height/2 );
                                        // Ativa a explosão
                                        explosion.IsOn = true;

                                        // Marca o acerto do Gundam
                                        gundam.Hit = true;

                                        // Subtrai 5 de vida do alvo
                                        t.Life -= 5;
                                        
                                    }

                                   // Aumenta a pontuação e reseta o alvo se este for destruído
                                if (t.Life <= 0)
                                    {
                                      // Fornece 500 pontos por meteoro e 10000 por genki dama             
                                    switch(t.Type)
                                        {
                                            case "meteor":
                                            {
                                                score += 500; break;
                                            }
                                            case "genkidama":
                                            { score += 10000; break; }
                              
                                        }
                                   
                                     t.Reset(difficulty.ToString());
      
                                    }

                                    // Testa colisões de meteoro com pirâmide
                                    if (t.Type == "meteor")
                                    {
                                        if (t.Position.Y + t.Size.Height >= pyramids[0].position.Y)
                                        {
                                            foreach (Sprite pyramid in pyramids)
                                            {

                                                if ((pyramid.Collision(t)) && (pyramid.Scale > 0f))
                                                {
                                                    
                                                    Rectangle OldSize = pyramid.Size;
                                                    Vector2 OldPosition = pyramid.Position;
                                                    explosion.IsOn = true;
                                                    Vector2 center = new Vector2(pyramid.size.Width / 2, pyramid.size.Height / 2);
                                                    // Diminui a pirâmide caso for acertada e ainda não tiver sumido
                                                    if (pyramid.Scale > 0)
                                                    {
                                                        pyramid.Scale -= 0.25f;
                                                    }
                                                    pyramid.Update(gameTime, Vector2.Zero);
                                                    pyramid.Position = new Vector2(OldPosition.X + OldSize.Width / 2 - pyramid.Size.Width / 2, OldPosition.Y + OldSize.Height / 2 - pyramid.Size.Height / 2);
                                                    explosion.Position = new Vector2(pyramid.Position.X + pyramid.Size.Width / 2 - explosion.Width / 2, pyramid.Position.Y + pyramid.Size.Height / 2 - explosion.Height / 2);
                                                    t.Reset(difficulty.ToString());
                                                    
                                                    // Subtrai uma vida caso uma pirâmide seja destruída
                                                    if (pyramid.Scale == 0f)
                                                    { counter -= 1; }
                                                }

                                            }


                                        }
                                        t.Update(gameTime, t.Speed);
                                    }

                                    // Testa colisões de Genki Damas com pirâmides
                                        if (t.Type == "genkidama")
                                        {
                                            // Ativa vibração se a Genki Dama estiver visível
                                            if ((t.Position.Y + t.Size.Height > 0)&&(t.Position.Y + t.Size.Height<pyramids[0].Position.Y))
                                            {
                                                GamePad.SetVibration(PlayerIndex.One, 0.2f, 0.2f);
                                            }
                                            t.Update(gameTime, t.Speed);                                                  
                                             foreach (Sprite thePyramid in pyramids)
                                                {
                                                 // Diminui cada pirâmide em um estágio e reseta caso acerte uma delas
                                                    if ((thePyramid.Collision(t)) && (thePyramid.Scale >= 0f))
                                                    {
                                                        foreach (Sprite pyramid in pyramids)
                                                        {
                                                            Rectangle OldSize = pyramid.Size;
                                                            Vector2 OldPosition = pyramid.Position;
                                                            explosion.IsOn = true;
                                                            Vector2 center = new Vector2(pyramid.size.Width / 2, pyramid.size.Height / 2);
                                                            if (pyramid.Scale > 0)
                                                            {
                                                                pyramid.Scale -= 0.25f;
                                                            }
                                                            pyramid.Update(gameTime, Vector2.Zero);
                                                            pyramid.Position = new Vector2(OldPosition.X + OldSize.Width / 2 - pyramid.Size.Width / 2, OldPosition.Y + OldSize.Height / 2 - pyramid.Size.Height / 2);
                                                            explosion.Position = new Vector2(pyramid.Position.X + pyramid.Size.Width / 2 - explosion.Width / 2, pyramid.Position.Y + pyramid.Size.Height / 2 - explosion.Height / 2);
                                                            if (pyramid.Scale == 0f)
                                                            { counter -= 1; }
                                                        }
                                                       // Aumenta a vibração caso haja um acerto
                                                         GamePad.SetVibration(PlayerIndex.One, 1f, 1f); 
                                                        t.Reset(difficulty.ToString()); 
                                                    }

                                                

                                            
                                            
                                        }
                                    }
                                     
                        }
                        #endregion

                        break;
                    }
                #endregion
                #region Options
                case GameState.Options:
                    {
                        // Zera a vibração do GamePad
                        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                        // Determina a dificuldade baseando-se em input
                        if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                            difficulty = Difficulty.Easy;
                        if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                            difficulty = Difficulty.Normal;
                        if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
                            difficulty = Difficulty.Hard;
                        if (Keyboard.GetState().IsKeyDown(Keys.NumPad9))
                            difficulty = Difficulty.Insane;

                        // Retorna para a tela inicial
                        if ((Keyboard.GetState().IsKeyDown(Keys.Escape))||(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back)))
                            gameState = GameState.Menu;
                        
                        break;
                    }
                #endregion
                #region Pause
                case GameState.Pause:
                    {
                        // Zera a vibração do GamePad
                        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                        // Dispensa os bancos de som para parar o som
                        if (!soundBank.IsDisposed)
                            soundBank.Dispose();
                        if (!musicBank.IsDisposed)
                            musicBank.Dispose();

                        // Retorna para o jogo
                        if ((Keyboard.GetState().IsKeyDown(Keys.Enter))||(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)))
                        {
                             gameState = GameState.Playing;
                        }
                        break;
                    }
                    
                #endregion
                #region Game Over
                case GameState.GameOver:
                    {
                        // Zera a vibração do GamePad
                        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                        // Cria novos bancos de som para tocar a música de Game Over
                        if (soundBank.IsDisposed)
                            soundBank = new SoundBank(audioEngine, "Sound Bank.xsb");
                        if (musicBank.IsDisposed)
                            musicBank = new SoundBank(audioEngine, "Sound Bank.xsb");

                        // Toca a música de Game Over caso o jogador tenha acabado de perder
                        if (lost)
                        {
                            soundBank.PlayCue("lose");
                            // Variável tornada falsa para evitar repetição da música
                           lost = false;
                        }
                        
                        // Retorna para o jogo
                        if((Keyboard.GetState().IsKeyDown(Keys.Enter))||(GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)))
                        {
                            soundBank.Dispose();
                            musicBank.Dispose();

                            // Diminui a dificuldade caso já não seja a menor
                            if (difficulty != Difficulty.Easy)
                            { difficulty -= 1; }

                            ResetGame();
                            gameState = GameState.Playing;
                        }
                        break;
                    }
                #endregion
            }

                base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            // Desenha diferentes itens na tela dependendo do estado atual do jogo
           switch(gameState)
           {
               #region Menu
               case GameState.Menu: 
{
    // Desenha a tela inicial
    spriteBatch.Draw(mainscreen, Vector2.Zero, Color.White);
    break; 
}
               #endregion
               #region Gameplay
               case GameState.Playing:
                   {
                       // Desenha o fundo de tela
                       spriteBatch.Draw(background, Vector2.Zero, backgroundRect, Color.White, 0.0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0);
                       
                       // Desenha todas as pirâmides
                       foreach (Sprite spr in pyramids)
                       {
                           spr.Draw(spriteBatch);
                       }

                       // Desenha o Goku
                       goku.Draw(spriteBatch);

                       // Desenha os alvos (apenas o meteoro tem rotação)
                           targets[0].Draw(spriteBatch,targets[0].Rotation);
                           targets[1].Draw(spriteBatch);
                       
                       // Desenha o escaravelho caso esteja sendo atirado
                       if (scarab.Shooting)
                       {
                           scarab.Draw(spriteBatch,scarab.Rotation);
                       }

                       // Desenha o escaravelho caso esteja sendo atirado
                       if (hadouken.Shooting)
                       {
                           hadouken.Draw(spriteBatch,hadouken.Rotation);
                       }

                       // Desenha o Gundam
                       gundam.Draw(spriteBatch);

                       // Desenha a explosão caso esteja acontecendo
                       if (explosion.IsGoing)
                       {
                           explosion.Draw(spriteBatch);
                       }
                     

                       // Desenha o placar
                       spriteBatch.DrawString(font, "Score:\n" + score.ToString(), new Vector2(670, 10), Color.Red);
                       break;
                   }
               #endregion
               #region Options
               case GameState.Options:
                   {
                       // Desenha as instruções para alterar dificuldade
                       spriteBatch.DrawString(font, "Choose the difficulty:\n Easy: Press 1\n Normal: Press 2\n Hard: Press 3\n\n Difficulty:"+difficulty.ToString(), new Vector2(250, 250), Color.Black);
                       break;
                   }
               #endregion
               #region Pause
               case GameState.Pause:
 {
     // Torna o fundo preto
     graphics.GraphicsDevice.Clear(Color.Black);

     // Desenha o aviso para retornar
     spriteBatch.DrawString(font, "Press Start to resume!", new Vector2(230, 300), Color.White);
     break;
 }
               #endregion
               #region Game Over
    case GameState.GameOver: {
        // Avisa que o jogo acabou e como voltar
        spriteBatch.DrawString(font, "Game Over", new Vector2(320, 300), Color.Red);
        spriteBatch.DrawString(font, "Press Start to try again", new Vector2(210, 400), Color.Red);
        break; }
    #endregion
}
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

  

        #region Reseter

        // Método chamado para resetar o jogo
        public void ResetGame()
        {
           // Reseta a posição do Gundam
            gundam.Position = gundam.InitialPosition;

            // Reseta o contador de vidas
            counter = 6;

            // Reseta o placar
            score = 0;

            // Reseta todas as pirâmides
            pyramids[0].Scale = 1.0f;
             pyramids[0].Position = new Vector2(20, this.GraphicsDevice.Viewport.Height - pyramids[0].Size.Height);
           for (int i = 1; i < 6; i++)
           {
               pyramids[i].Scale = 1.0f;
               pyramids[i].position.X = pyramids[i - 1].Position.X + pyramids[i].Size.Width;
               pyramids[i].position.Y = pyramids[i - 1].position.Y;
               
           }

            // Reseta os alvos
           foreach (Target t in targets)
           {
               t.Reset(difficulty.ToString());
           }
       }
        #endregion
        #region Projectile Methods

       // Métodos para preparar os projéteis
            
         public void Shoot(Projectile projectile, Vector2 theSpeed, string theCue)
        {
            /* Método para disparo de Mouse
             */
            // Faz o som do projétil
            soundBank.PlayCue(theCue);

            // Começa o processo de tiro
            projectile.Shooting = true;

            // Determina a posição inicial do tiro
            projectile.position.X = gundam.Position.X + gundam.Width / 2;
            projectile.position.Y = gundam.Position.Y + gundam.Height / 2;

            double XCoordinate, YCoordinate;
            // Distância entre o mouse e a posição inicial do projétil no eixo X
            XCoordinate = (double)Mouse.GetState().X - projectile.Position.X;

            // Distância entre o mouse e a posição inicial do projétil no eixo Y
            YCoordinate = (double)Mouse.GetState().Y - projectile.Position.Y;
           
            // Determina o ângulo em que o projétil será atirado, que é o mesmo da tangente entre as coordenadas citadas
            projectile.Rotation = (float)Math.Atan2(YCoordinate, XCoordinate);

            // Coloca a velocidade fornecida no projétil
            projectile.Speed = theSpeed;

            // Desativa o estágio inicial do tiro
            projectile.Shoot = false;
        }
            
          public void Shoot(Projectile projectile, Vector2 theSpeed, string theCue, Vector2 thumbPosition)
        {
            /* Método para disparo de GamePad
          */
              // Toca o som do projétil
            soundBank.PlayCue(theCue);
              // Inicia o processo de tiro
            projectile.Shooting = true;

              // Determina posição inicial do projétil
            projectile.position.X = gundam.Position.X+gundam.Width/2;
            projectile.position.Y = gundam.Position.Y+gundam.Height/2;

              // Determina direção do projétil baseado na alavanca do GamePad
            projectile.Rotation = (float)Math.Atan2(thumbPosition.Y*-1, thumbPosition.X);

              // Determina a velocidade do projétil
            projectile.speed.X = theSpeed.X ;
            projectile.speed.Y = theSpeed.Y ;

              // Desativa o estágio inicial do tiro
            projectile.Shoot = false;
        }

        // Método chamado durante o disparo do projétil
        public void Shooting(Projectile projectile, List<Target>theTargets,GameTime gameTime)
        {
            // Atualiza o projétil caso esteja sendo atirado
            projectile.Update(gameTime, new Vector2(projectile.Speed.X, projectile.Speed.Y), projectile.Rotation);

            // Checa colisões dos alvos com os projéteis
            foreach(Target t in theTargets)
            if ((projectile.Collision(t)))
            {
                // Determina a posição da explosão (mesma do projétil)
                explosion.Position = new Vector2(projectile.Position.X, projectile.Position.Y);

                // Inicia a explosão
                explosion.IsOn = true;
                // Aumenta 100 pontos por acerto
                score += 100;
                switch (projectile.Type)
                {
                    case "scarab":
                        {
                            // Subtrai 1 de vida do alvo caso o projétil seja um escaravelho
                            t.Life -= 1;
                            break;
                        }
                    case "hadouken":
                        {
                            // Subtrai 30 de vida do alvo caso o projétil seja um Hadouken
                            t.Life -= 30;
                            break;
                        }
                }
                // Desativa o processo de tiro caso haja colisão
                projectile.Shooting = false;

            }

            // Desativa o processo de tiro caso o projétil saia da tela
        if (projectile.OutOfSight(graphics.GraphicsDevice))
            {
                projectile.Shooting = false;
            }
        }
       #endregion

        #region Difficultator
        // Método que aumenta dificuldade baseado no número de pontos
        public void DifficultyUp()
        {
            // Caso o placar passe de 1000000, ativar dificuldade insana 
            if (score >= 1000000)
            { difficulty = Difficulty.Insane; }
                // Caso o placar seja menor que 1000000 mas maior que 200000, aumenta a dificuldade
            else if (score >= 200000)
            {
                switch (difficulty)
                {
                    case Difficulty.Normal: { difficulty = Difficulty.Hard; break; }
                }

            }
            // Caso o placar passe de 50000, aumenta a dificuldade 
            if (score >= 50000)
            {
                switch (difficulty)
                {
                    case Difficulty.Easy: { difficulty = Difficulty.Normal; break; }
                    case Difficulty.Normal: { difficulty = Difficulty.Hard; break; }
                }
            }
        }
        #endregion
    }
}
