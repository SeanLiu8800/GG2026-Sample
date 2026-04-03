using UnityEngine;
using System.Collections;
public class BulletLifespan : BulletComponent
{
    [SerializeField, Range(0.0f, 3.0f)] private float lifespan = 0.2f;

    private void Start()
    {
        StartCoroutine(ManageLifespan());
    }
    private IEnumerator ManageLifespan()
    {
        if (lifespan <= 0.0) Debug.LogWarning($"{this.name}'s lifespan is 0.0! Bullet will be destroyed immediately!");

        StartCoroutine(FadeSpriteHelper.FadeCoroutine(bullet.GetComponent<SpriteRenderer>(), 0, lifespan));

        float spawnTime = Time.time;
        while (Time.time - spawnTime < lifespan) yield return null;

        Destroy(this.gameObject);
    }
}
