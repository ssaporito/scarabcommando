using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scarab_Commando
{
    class Gundam:Animation
    {
        // Declara os possíveis estados do Gundam(Parado, Andando, Pulando, Caindo, Atacando, Pulando e Atacando)
        public enum State { Still, Walking, Jumping, Falling, Attacking, JumpAttacking }
  
        bool orientationX, hit;

       
        public bool OrientationX
        {
            // Sentido do Gundam ( falso = esquerda, verdadeiro = direita)
            get { return orientationX; }
            set { orientationX = value; }
        }
        public bool Hit
        {
            // Determina se o Gundam está acertando algum alvo com a espada
            get { return hit; }
            set { hit = value; }
        }

        // Estado atual do Gundam
        public State state;

        // Altura máxima a que o Gundam pode chegar durante um pulo
        protected float maxY;
        public float MaxY
        { get { return maxY; } set { maxY = value; } }
        

        // Determina os frames corretos do Gundam de acordo com seu sentido e com seu estado atual
        public void Animate(GameTime gameTime)
        {
            switch (OrientationX)
            {
                case true:
                    { this.Speed = new Vector2(500, 0); }
                    break;
                case false:
                    { this.Speed = new Vector2(-500, 0); }
                    break;
            }
       
                        switch (state)
                        {
                            #region Still
                            case State.Still: { 
                                // Escolhe o frame para ficar parado olhando para a direita
                                CurrentFrame = 0; break; }
                            #endregion

                            #region Walking
                            case State.Walking:
                                {
                                    // Anda para a direita
                                    Update(gameTime, Speed);

                                    // Diminui o tempo restante até o frame seguinte
                                    Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                                    // Troca o frame quando o tempo necessário passa
                                    if (Timer > Interval)
                                    {

                                        CurrentFrame++;

                                        
                                        if (CurrentFrame > 3)
                                        {

                                            CurrentFrame = 1;
                                        }
                                        // Zera o timer a cada troca de frame
                                        Timer = 0f;
                                    }
                                    break;
                                }
                        #endregion
                            #region Jumping
                            case State.Jumping:
                                {
                                    // Determina se o sprite se movimentará lateralmente durante o pulo caso a altura máxima ainda não tenha sido atingida
                                    if (this.Position.Y > this.MaxY)
                                    {
                                        if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)) || (Keyboard.GetState().IsKeyDown(Keys.D)))
                                            Update(gameTime, new Vector2(500, -800));
                                        else if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (Keyboard.GetState().IsKeyDown(Keys.A)))
                                            Update(gameTime, new Vector2(-500, -800));
                                        else Update(gameTime, new Vector2(0, -800));
                                    }
                                    
                                        // Caso a altura máxima tenha sido atingida, começa a cair
                                    else
                                    { state = State.Falling; }

                                    // Muda o sentido do Gundam baseado em input
                                    if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)))
                                    { OrientationX = false; }

                                    Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


                                    if (Timer > Interval)
                                    {

                                        if (CurrentFrame != 5)
                                        {
                                            CurrentFrame = 5;
                                        }

                                        Timer = 0f;
                                    }

                                    // Testa se o botão de ataque foi pressionado
                                    if (((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))||(Mouse.GetState().RightButton==ButtonState.Pressed)))
                                    {
                                        // Muda para o estado de pulo e ataque
                                        CurrentFrame = 6; state = State.JumpAttacking;
                                    }
                                    break;
                                }
                            #endregion
                            #region Falling
                            case State.Falling:
                                {
                                    // Determina se o sprite se movimentará lateralmente durante a queda caso a altura mínima ainda não tenha sido atingida
                                    if (this.Position.Y < this.InitialPosition.Y)
                                    {
                                        if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))||(Keyboard.GetState().IsKeyDown(Keys.D)))
                                            this.Update(gameTime, new Vector2(500, 600));
                                        else if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))||(Keyboard.GetState().IsKeyDown(Keys.A)) )
                                            this.Update(gameTime, new Vector2(-500, 600));
                                        else this.Update(gameTime, new Vector2(0, 600));
                                    }
                         // Caso a altura mínima (chão) tenha sido atingida, Gundam fica parado
                                    else
                                    {
                                        state = State.Still;
                                    }
                                    if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)))
                                    { this.OrientationX = false; }
                                    break;
                                }
                            #endregion
                            #region Attacking
                            case State.Attacking:
                                {
                                    Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


                                    if (Timer >Interval)
                                    {

                                        CurrentFrame++;

                                        if (CurrentFrame > 13)
                                        {

                                            CurrentFrame = 9;
                                            // Só desativa o acerto após o fim da animação, para evitar múltiplas colisões
                                            Hit = false;
                                            // Fica parado após o ataque
                                            state = State.Still;
                                        }

                                        Timer = 0f;
                                    }

                                    break;
                                }
                            #endregion
                            #region JumpAttacking
                            case State.JumpAttacking:
                                {

                                    // Vira para a esquerda caso botões de movimento para a esquerda sejam pressionados durante o pulo com ataque
                                    if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)))
                                    { OrientationX = false; }

                                    Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                                    if (Timer > Interval)
                                    {
                                        CurrentFrame++;
                                        if (CurrentFrame > 8)
                                        {
                                            CurrentFrame = 6;
                                            // Só desativa o acerto após o fim da animação, para evitar múltiplas colisões
                                            Hit = false;
                                            // Cai após o ataque
                                            state = State.Falling;
                                        }

                                        Timer = 0f;
                                    }
                                    break;
                                }
                        #endregion
                        }
                        
                

            SourceRect = new Rectangle(CurrentFrame * Width, 0, Width, Height);

            DestRect = new Rectangle((int)position.X, (int)position.Y, Width, Height);
        }

        // Desenha o retângulo que percorre a imagem(SourceRect) no retângulo da tela(DestRect)
        public void Draw(SpriteBatch spriteBatch)
        {
            switch(OrientationX)
            {
                case true:
                    {
                        spriteBatch.Draw(this.mSpriteTexture, DestRect, SourceRect, Color.White,0f,Vector2.Zero,SpriteEffects.None,0);
                    }
                    break;
                case false:
                    {
                        spriteBatch.Draw(this.mSpriteTexture, DestRect, SourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    }
                    break;
        }
        }

        // Testa se o Gundam saiu da tela e impede o movimento neste caso
        public void TestOutOfSight(GraphicsDevice g)
        {
            if (this.Position.X + this.Width > g.Viewport.Width)
            { this.position.X = g.Viewport.Width - this.Width; }

            if (this.Position.X < 0)
            { this.position.X = 0; }

            if (this.Position.Y + this.Height > g.Viewport.Height)
            { this.position.Y = g.Viewport.Height - this.Height; }

            if (this.Position.Y < 0)
            { this.position.Y = 0; }
        }
    }
}
