using System;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

[Serializable]
public struct Rectangle
{
    public Vector2 leftRight;
    public Vector2 bottomUp;
    
    public Rectangle(Vector2 leftRight, Vector2 bottomUp)
    {
        this.leftRight = leftRight;
        this.bottomUp = bottomUp;
    }
    
    public float bottom
    {
        get => bottomUp.x;
        set => bottomUp = new Vector2(value, bottomUp.y);
    }

    public float up
    {
        get => bottomUp.y;
        set => bottomUp = new Vector2(bottomUp.x, value);
    }

    public float left
    {
        get => leftRight.x;
        set => leftRight = new Vector2(value, leftRight.y);
    }

    public float right
    {
        get => leftRight.y;
        set => leftRight = new Vector2(leftRight.x, value);
    }
    
    public Vector2 Center => new Vector2((left + right)/2f, (up + bottom)/2f);
    public Vector2 Size => new Vector2((right - left), (up - bottom));
    
    // 判断一个点是否在该矩形框内, 要考虑矩形框是一条线的情况
    public bool IsPointInRectangle(Vector2 point)
    {
        if (right == left && bottom == up)
        {
            return false;
        }
        
        if (bottom == up)
        {
            return point.x >= left && point.x <= right;
        }

        if (left == right)
        {
            return point.y >= bottom && point.y <= up;
        }
        
        return point.x >= left && point.x <= right && point.y >= bottom && point.y <=
            up;
    }

    public bool IsPointInRangeX(Vector2 point)
    {
        if (left == right)
            return false;
        return point.x >= left && point.x <= right;
    }

    public bool IsPointInRangeX(float x)
    {
        if (left == right)
            return false;
        return x >= left && x <= right;
    }

    public bool IsPointInRangeY(Vector2 point)
    {
        if (bottom == up)
            return false;
        return point.y >= bottom && point.y <= up;
    }

    public bool IsPointInRangeY(float y)
    {
        if (bottom == up)
            return false;
        return y >= bottom && y <= up;
    }
    
    // 当一个点在矩形框外时, 算出该点到矩形框边缘最近的位置
    public Vector2 ClosestPositionOnEdge(Vector2 point)
    {
        if (point.x < left)
            point.x = left;
        else if (point.x > right)
            point.x = right;

        if (point.y < bottom)
            point.y = bottom;
        else if (point.y > up)
            point.y = up;

        return point;
    }

    public Rectangle ConvertToWorldSpace(Vector2 pivot)
    {
        return new Rectangle(
            new Vector2(pivot.x+left, pivot.x+right),
            new Vector2(pivot.y+bottom, pivot.y+up)
            );
    }
}

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool debug = false;
    [SerializeField] private bool debugGroundedOrAirborne = false;

    #region References
    
    private Camera camera;
    [Space, Header("Reference Component")]
    [SerializeField] private Transform target;
    [SerializeField] private Player player;
    [SerializeField] private InputController input;

    #endregion

    #region Camera Move Parameters

    [Space, Header("Camera Move Parameters")]
    [SerializeField] private Vector2 offset = new Vector2(0, 1);
    [SerializeField] private float softSmoothTime = 1f;
    [SerializeField] private float sensitiveSmoothTimeX = 0.1f;
    [SerializeField] private float sensitiveSmoothTimeY = 0.1f;

    [Space] [SerializeField] private float maxSpeedX = 9f;
    [SerializeField] private float maxSpeedY = 9f;
    
    [Space] [SerializeField] private Vector2 softXRangeGrounded = new Vector2(-0.5f, 0.5f);
    [SerializeField] private Vector2 freeYRangeAirborne = new Vector2(0f, 2.5f);
    [SerializeField] private Rectangle sensitiveRange = new Rectangle(
        new Vector2(-6.5f, 6.5f),
        new Vector2(-3.5f, 4f)
    );
    
    #endregion

    #region Camera Control
    
    [Space, Header("Camera Control")]
    [SerializeField] private float observeUp = 1f;
    [SerializeField] private float observeDown = -2f;
    [SerializeField] private float timeToObservePos = 0.3f;
    
    #endregion

    #region Runtime Parameters

    // Move
    private Vector2 targetPosition;
    private Vector3 _refVelocity;
    private float distanceZ;
    
    // Control
    private float observeInputValue;
    private int observePos = 0; // -1表示向下; 0表示原位置; 1表示向上
    private float observeTime = 0f;
    private bool isObserveMoving = false;
    private float startObserveY;
    private float targetObserveY;
    
    #endregion
    
    private void Awake()
    {
        camera = GetComponent<Camera>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (target == null)
            target = playerObj.transform;
        if (player == null)
            player = playerObj.GetComponent<Player>();
        if (input == null)
            input = playerObj.GetComponent<InputController>();
    }

    private void Start()
    {
        distanceZ = transform.position.z;
    }

    private void Update()
    {
        input.CheckObserveInput(ref observeInputValue);   
    }

    private void LateUpdate()
    {
        // 摄像机追随的目标点
        targetPosition = new Vector2(target.position.x, target.position.y) + offset;
        
        // 将设定的范围值转换到世界坐标
        Vector2 pivot = new Vector2(transform.position.x, transform.position
            .y);
        Vector2 softXRangeGroundedWS = new Vector2(pivot.x +
                                                   softXRangeGrounded.x,
            pivot.x + softXRangeGrounded.y);
        Vector2 freeYRangeAirborneWS = new Vector2(pivot.y +
                                                   freeYRangeAirborne.x,
            pivot.y + freeYRangeAirborne.y);
        Rectangle sensitiveRangeWS = sensitiveRange.ConvertToWorldSpace(pivot);

        float x = transform.position.x;
        float y = transform.position.y;
        
        
        if (player.IsOnGround)
        {
            // 追随目标点的逻辑
            if (targetPosition.x >= softXRangeGroundedWS.x && targetPosition
                    .x <= softXRangeGroundedWS.y)
            {
                x = Mathf.SmoothDamp(transform.position.x, targetPosition.x,
                    ref _refVelocity.x, softSmoothTime);
            }
            else if (sensitiveRangeWS.IsPointInRangeX(targetPosition.x))
            {
                x = Mathf.SmoothDamp(transform.position.x, targetPosition.x,
                    ref _refVelocity.x, sensitiveSmoothTimeX, maxSpeedX);
            }
            else
            {
                if ( targetPosition.x < sensitiveRangeWS.left)
                {
                    x = sensitiveRangeWS.left;
                }
                else if (targetPosition.x > sensitiveRangeWS.right)
                {
                    x = sensitiveRangeWS.right;
                }
            }
            
            // 上下观察的逻辑
            CheckObservePos();
            
            // 在非观察模式下, 才进行上下跟踪
            if (observePos == 0 && !isObserveMoving)
            {
                if (sensitiveRangeWS.IsPointInRangeY(targetPosition.y))
                {
                    y = Mathf.SmoothDamp(transform.position.y, targetPosition.y,
                        ref _refVelocity.y, sensitiveSmoothTimeY, maxSpeedY);
                }
            }
            
            // 执行观察镜头移动
            if (isObserveMoving)
            {
                y = Mathf.Lerp(startObserveY, targetObserveY,
                    observeTime / timeToObservePos);
                observeTime += Time.deltaTime;
                if (observeTime > timeToObservePos)
                {
                    isObserveMoving = false;
                    y = targetObserveY;
                }
            }
            
            if (targetPosition.y < sensitiveRangeWS.bottom)
            {
                y = sensitiveRangeWS.bottom;
            }
            else if (targetPosition.y > sensitiveRangeWS.up)
            {
                y = sensitiveRangeWS.up;
            }
        }
        else
        {
            observePos = 0;
            isObserveMoving = false;
            
            if (sensitiveRangeWS.IsPointInRangeX(targetPosition.x))
            {
                x = Mathf.SmoothDamp(transform.position.x, targetPosition.x,
                    ref _refVelocity.x, sensitiveSmoothTimeX, maxSpeedX);
            }
            else
            {
                if ( targetPosition.x < sensitiveRangeWS.left)
                {
                    x = sensitiveRangeWS.left;
                }
                else if (targetPosition.x > sensitiveRangeWS.right)
                {
                    x = sensitiveRangeWS.right;
                }
            }

            if ( targetPosition.y > freeYRangeAirborneWS.x && targetPosition
                .y < freeYRangeAirborneWS.y )
            {
                y = transform.position.y;
            }
            else if ( sensitiveRangeWS.IsPointInRangeY(targetPosition.y) )
            {
                y = Mathf.SmoothDamp(transform.position.y, targetPosition.y,
                    ref _refVelocity.y, sensitiveSmoothTimeY, maxSpeedY);
            }
            else
            {
                if (targetPosition.y < sensitiveRangeWS.bottom)
                {
                    y = sensitiveRangeWS.bottom;
                }
                else if (targetPosition.y > sensitiveRangeWS.up)
                {
                    y = sensitiveRangeWS.up;
                }
            }
        }
        
        transform.position = new Vector3(x, y, distanceZ);
    }
    
    // 设定观察位置(observePos), 设定观察移动Lerp的目标值
    void CheckObservePos()
    {
        if (observePos == 0)
        {
            if (observeInputValue <= -0.1f)
            {
                SetObserveTargetY(-1);
            }
            else if (observeInputValue >= 0.1f)
            {
                SetObserveTargetY(1);
            }
        }
        else if (observePos == -1)
        {
            if (Mathf.Abs(observeInputValue) < 0.1f)
            {
                SetObserveTargetY(0);
            }
            else if (observeInputValue >= 0.1f)
            {
                SetObserveTargetY(1);
            }
        }
        else if (observePos == 1)
        {
            if (Mathf.Abs(observeInputValue) < 0.1f)
            {
                SetObserveTargetY(0);
            }
            else if (observeInputValue <= -0.1f)
            {
                SetObserveTargetY(-1);
            }
        }
        
        if (Mathf.Abs(observeInputValue) < 0.1f)
        {
            observePos = 0;
        }
        else if (observeInputValue < 0)
        {
            observePos = -1;
        }
        else
        {
            observePos = 1;
        }
    }

    
    void SetObserveTargetY(int observePos)
    {
        observeTime = 0f;
        isObserveMoving = true;
        startObserveY = transform.position.y;
        if (observePos == 0)
        {
            targetObserveY = targetPosition.y;
        }
        else if (observePos == -1)
        {
            targetObserveY = targetPosition.y + observeDown;
        }
        else if (observePos == 1)
        {
            targetObserveY = targetPosition.y + observeUp;
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0.8f, 0.3f, 0.4f);
            Gizmos.DrawWireCube(sensitiveRange.Center, sensitiveRange.Size);
            
            // 绘制左右的softRange
            Gizmos.color = new Color(0.8f, 0.8f, 0.4f);
            Gizmos.DrawLine(new Vector3(softXRangeGrounded.x,0,0), new 
                Vector3(softXRangeGrounded.y,0,0));
            Gizmos.DrawLine(new Vector3(softXRangeGrounded.x, -0.2f, 0), new
                Vector3(softXRangeGrounded.x, 0.2f, 0));
            Gizmos.DrawLine(new Vector3(softXRangeGrounded.y, -0.2f, 0), new
                Vector3(softXRangeGrounded.y, 0.2f, 0));
            
            // 绘制上下的freeRange
            Gizmos.color = new Color(0.2f, 0.7f, 0.3f);
            Gizmos.DrawLine(new Vector3(0,freeYRangeAirborne.x,0),
                new Vector3(0,freeYRangeAirborne.y,0)
                );
            Gizmos.DrawLine(new Vector3(-0.2f,freeYRangeAirborne.x,0),
                new Vector3(0.2f,freeYRangeAirborne.x, 0)
            );
            Gizmos.DrawLine(new Vector3(-0.2f,freeYRangeAirborne.y,0),
                new Vector3(0.2f,freeYRangeAirborne.y,0)
            );
            
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
    
}
