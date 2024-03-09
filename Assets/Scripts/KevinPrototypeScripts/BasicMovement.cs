namespace AE0672
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BasicMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        [SerializeField] AudioSource hitSound;

        private void Start()
        {
            hitSound = GetComponent<AudioSource>();
        }
        

        void Update()
        {

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;


            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;


            cameraForward.y = 0f;
            cameraRight.y = 0f;


            cameraForward.Normalize();
            cameraRight.Normalize();


            Vector3 movementDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;


            transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                Debug.Log("Enemy hit");
                hitSound.Play();

                StartCoroutine(DestroyEnemy(collision.gameObject));

            }
        }

        IEnumerator DestroyEnemy(GameObject enemy)
        {

            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.red;
            }


            yield return new WaitForSeconds(2f);


            Destroy(enemy);
        }
    }

}