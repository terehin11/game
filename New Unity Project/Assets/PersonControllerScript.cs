using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonControllerScript : MonoBehaviour
{
    public float maxSpeed = 20f;
    public bool isFacingRight = true; //направление персонажа влево/вправо
    private Animator anim; //ссылка на компонент анимаций
    private Rigidbody2D rigidbody2D;
    //находится ли персонаж на земле или в прыжке?
    private bool isGrounded = false;
    //ссылка на компонент Transform объекта
    //для определения соприкосновения с землей
    public Transform groundCheck;
    //радиус определения соприкосновения с землей
    private float groundRadius = 0.2f;
    //ссылка на слой, представляющий землю
    public LayerMask whatIsGround;
    // Start is called before the first frame update
    /// <summary>
    /// Начальная инициализация
    /// </summary>
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Выполняем действия в методе FixedUpdate, т. к. в компоненте Animator персонажа
    /// выставлено значение Animate Physics = true и анимация синхронизируется с расчетами физики
    /// </summary>
    private void FixedUpdate()
    {
        //определяем, на земле ли персонаж
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        //устанавливаем соответствующую переменную в аниматоре
        anim.SetBool("Ground", isGrounded);
        //устанавливаем в аниматоре значение скорости взлета/падения
        anim.SetFloat("vSpeed", rigidbody2D.velocity.y);
        //если персонаж в прыжке - выход из метода, чтобы не выполнялись действия, связанные с бегом
        if (!isGrounded)
            return;
        //используем Input.GetAxis для оси Х. метод возвращает значение оси в пределах от -1 до 1.
        //при стандартных настройках проекта 
        //-1 возвращается при нажатии на клавиатуре стрелки влево (или клавиши А),
        //1 возвращается при нажатии на клавиатуре стрелки вправо (или клавиши D)
        float move = Input.GetAxis("Horizontal");
        //в компоненте анимаций изменяем значение параметра Speed на значение оси Х.
        //приэтом нам нужен модуль значения
        anim.SetFloat("Speed", Mathf.Abs(move));
        //обращаемся к компоненту персонажа RigidBody2D. задаем ему скорость по оси Х, 
        //равную значению оси Х умноженное на значение макс. скорости

        rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);
        //если нажали клавишу для перемещения вправо, а персонаж направлен влево
        if (move > 0 && !isFacingRight)
        {//отражаем персонажа вправо
            Flip();
        }
        else if (move < 0 && isFacingRight) Flip();

    }
    /// <summary>
    /// Метод для смены направления движения персонажа и его зеркального отражения
    /// </summary>
    private void Flip()
    {
        //меняем направление движения персонажа 
        isFacingRight = !isFacingRight;
        //получаем разрмеры персонажа
        Vector3 theScale = transform.localScale;
        //зеркольно отражаем персонажа по оси Х
        theScale.x *= -1;
        //задаем навый размер персонажа, равный старому, но зеркально отраженный
        transform.localScale = theScale; 

     }

    // Update is called once per frame
    void Update()
    {
        //если персонаж на земле и нажат пробел...
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //устанавливаем в аниматоре переменную в false
            anim.SetBool("Ground", false);
            //прикладываем силу вверх, чтобы персонаж подпрыгнул
            rigidbody2D.AddForce(new Vector2(0, 600));
        }
    }
}
