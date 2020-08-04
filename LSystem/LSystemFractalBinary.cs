	//F: draw a line segment
	//X: draw a line segment ending in a leaf
	//[: push position and angle, turn left 45 degrees
	//]: pop position and angle, turn right 45 degrees

	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	public class LSystemFractalBinary : MonoBehaviour
	{
		public struct CachedTransform
		{
			public Vector3 position;
			public Vector3 rotation;
		}

		[SerializeField]
		private float iterations = 0;

		[SerializeField]
		private float angle = 25f;

		[SerializeField]
		private TreeElement treeElement;

		[SerializeField]
		private Transform treeParent;

		[SerializeField]
		private string axiom = "X";

		[SerializeField]
		private float segmentWidth = 0.5f;

		private Vector3 posInit;

		private Dictionary<char, string> rules = new Dictionary<char, string>();

		private string currenAxiom;

		private Stack<CachedTransform> transformStack = new Stack<CachedTransform>();

		public void GenerateAxiom()
		{
			for (int i = 0; i < iterations; i++)
			{
				string evolutionAxiom = string.Empty;
				for (int j = 0; j < currenAxiom.Length; j++)
				{
					string aux = rules.ContainsKey(currenAxiom[j]) ? rules[currenAxiom[j]] : currenAxiom[j].ToString();
					evolutionAxiom = string.Concat(evolutionAxiom, aux);
				}
				currenAxiom = evolutionAxiom;
			}

			StartCoroutine(ShowTree());
		}

		public IEnumerator ShowTree()
		{
			Debug.Log(currenAxiom);

			segmentWidth *= 1 / iterations;

			for (int i = 0; i < currenAxiom.Length; i++)
			{
				switch (currenAxiom[i])
				{
					case 'F':
						TreeElement aux = Instantiate(treeElement);
						aux.transform.SetParent(treeParent);
						aux.transform.position = transform.position;
						aux.lineRenderer.positionCount = 2;
						aux.lineRenderer.SetPosition(0, transform.position);

						Vector2 newPosition = transform.position + transform.up * segmentWidth;
						aux.lineRenderer.SetPosition(1, newPosition);
						transform.position = newPosition;
						break;

					case 'X':
						TreeElement aux2 = Instantiate(treeElement);
						aux2.transform.SetParent(treeParent);
						aux2.transform.position = transform.position;
						aux2.lineRenderer.positionCount = 2;
						aux2.lineRenderer.SetPosition(0, transform.position);

						Vector2 newPosition2 = transform.position + transform.up * segmentWidth;
						aux2.lineRenderer.SetPosition(1, newPosition2);
						transform.position = newPosition2;

						aux2.lineRenderer.material.color = Color.cyan;
						break;

					case '[':
						transformStack.Push(new CachedTransform
						{
							position = transform.position,
								rotation = transform.eulerAngles
						});

						transform.Rotate(Vector3.forward * angle);
						break;

					case ']':
						CachedTransform cached = transformStack.Pop();
						transform.position = cached.position;
						transform.eulerAngles = cached.rotation;

						transform.Rotate(Vector3.forward * -angle);
						break;
				}
				yield return new WaitForEndOfFrame();
			}
		}

		public void Awake()
		{
			rules.Clear();
			rules.Add('F', "FF");
			rules.Add('X', "F[X]X");

			transformStack.Clear();

			currenAxiom = axiom;

			GenerateAxiom();
		}
	}