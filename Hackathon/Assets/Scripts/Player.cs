using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Allows us to use dictionaries

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    public float restartLevelDelay = .1f;        //Delay time in seconds to restart level.
    public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
    public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
    public int enemyDamage = 1;                 //How much damage a player does to an enemy when attacking it.
    
    
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    private int hp;                             //Used to store player health points total during level.
    private bool skipHealth;                    //Regenerate health only every other turn


    public Dictionary<string, int> inventory;   //Store the number of each drop we have.
    
    
    //Start overrides the Start function of MovingObject
    protected override void Start ()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();
        
        //Get the current health point total stored in GameManager.instance between levels.
        hp = GameManager.instance.playerHealthPoints;

        //Get the current inventory from the dictionary stared in GameManager.instance between scenes.
        inventory = GameManager.instance.playerInventory;
        
        //Call the Start function of the MovingObject base class.
        base.Start ();
    }
    
    
    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable ()
    {
        //When Player object is disabled, store the current local health total in the GameManager so it can be re-loaded in next level.
        GameManager.instance.playerHealthPoints = hp;

        //Store the current inventory in the GameManager to be reloaded later
        GameManager.instance.playerInventory = inventory;
    }
    
    
    private void Update ()
    {
        //If it's not the player's turn, exit the function.
        if(!GameManager.instance.playersTurn) return;
        
        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.
        
        
        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
        
        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int) (Input.GetAxisRaw ("Vertical"));
        
        //Check if moving horizontally, if so set vertical to zero.
        //if(horizontal != 0)
        //{
        //    vertical = 0;
        //}
        
        if (horizontal < 0)
        {
            vertical = 0;
            animator.SetTrigger("moveLeft");
        }

        else if (horizontal > 0)
        {
            vertical = 0;
            animator.SetTrigger("moveRight");
        }

        else if (vertical < 0)
        {
            horizontal = 0;
            animator.SetTrigger("moveFront");
        }

        else if (vertical > 0)
        {
            horizontal = 0;
            animator.SetTrigger("moveBack");
        }

        //Check if we have a non-zero value for horizontal AND vertical,
        //Set both to zero to prevent diagonal movement.
        if(horizontal !=0 && vertical != 0)
        {
            horizontal = 0;
            vertical = 0;
        }

        //Check if we have a non-zero value for horizontal or vertical
        if(horizontal != 0 || vertical != 0)
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Enemy> (horizontal, vertical);
        }
    }
    
    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        Debug.Log("AttemptMove Player called");

        //Regenerate health every other step
        if (!skipHealth) {
            hp++;
            skipHealth = true;
        }

        else {
            skipHealth = false;
        }
        
        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove <T> (xDir, yDir);
        
        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;
        
        //If Move returns true, meaning Player was able to move into an empty space.
        if (Move (xDir, yDir, out hit)) 
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
        }
        
        //Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver ();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }
    
    
    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is an Enemy which the player can attack and kill.
    protected override void OnCantMove <T> (T component)
    {
        Debug.Log("OnCantMove Player called");
        //Set hitEnemy to equal the component passed in as a parameter.
        Enemy hitEnemy = component as Enemy;
        
        //Call the DamageEnemy function of the Enemy we are hitting.
        hitEnemy.DamageEnemy (enemyDamage);
        
        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        //animator.SetTrigger ("playerChop");
    }
    
    
    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Untagged")
            return;

        //Check if the tag of the trigger collided with is the house.
        else if(other.tag == "House")
        {
            GameManager.level = 0;
            //transform.position = new Vector2(0f, 0f);
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);
            //Application.LoadLevel("Inside");
            
            // //Disable the player object since level is over.
            // enabled = false;
        }

        else if(other.tag == "Exit")
        {
            GameManager.level = 1;
            //transform.position = new Vector2(5, 5);
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);
            //Application.LoadLevel("Outside");
            
            // //Disable the player object since level is over.
            // enabled = false;
        }
        
        else if(other.tag == "Cook")
        {
            // Pop up the maker menu!
            GameObject CookCanvas = GameManager.instance.CookCanvas;
            GameObject Cookbutton = GameManager.instance.CookButton;
            CookCanvas.SetActive(true);
        }

        //Check if the tag of the trigger collided with is a drop.
        // else if(other.tag == "Drop")
        // {
        //     // //Add pointsPerFood to the players current food total.
        //     // food += pointsPerFood;
            
        //     //Disable the food object the player collided with.
        //     other.gameObject.SetActive (false);
        // }

        //Else, add the drop item to the inventory.
        else if (inventory.ContainsKey(other.tag))
        {
            inventory[other.tag] += 1;

            //Disable the drop item the player collided with.
            other.gameObject.SetActive (false);

            Debug.Log("Inventory: " + other.tag + inventory[other.tag]);
        }

        else if (!inventory.ContainsKey(other.tag))
        {
            inventory[other.tag] = 1;

            //Disable the drop item the player collided with.
            other.gameObject.SetActive (false);

            Debug.Log("Inventory: " + other.tag + inventory[other.tag]);
        }
        
        // //Check if the tag of the trigger collided with is Soda.
        // else if(other.tag == "Soda")
        // {
        //     //Add pointsPerSoda to players food points total
        //     food += pointsPerSoda;
            
            
        //     //Disable the soda object the player collided with.
        //     other.gameObject.SetActive (false);
        // }
    }
    
    
    //Restart reloads the scene when called.
    private void Restart ()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        Application.LoadLevel (Application.loadedLevel);
    }
    
    
    //LoseHealth is called when an enemy attacks the player.
    //It takes a parameter loss which specifies how many points to lose.
    public void LoseHealth (int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        //animator.SetTrigger ("playerHit");
        
        //Subtract lost health points from the players total.
        hp -= loss;
        
        //Check to see if game has ended.
        CheckIfGameOver ();
    }
    
    
    //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void CheckIfGameOver ()
    {
        //Check if food point total is less than or equal to zero.
        if (hp <= 0) 
        {
            
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver ();
        }
    }
}