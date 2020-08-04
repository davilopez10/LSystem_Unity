	//F: draw a line segment
	//X: draw a line segment ending in a leaf
	//[: push position and angle, turn left 45 degrees
	//]: pop position and angle, turn right 45 degrees

	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	public class LSystemFractalPlant : MonoBehaviour
	{
		public struct CachedTransform
		{
			public Vector3 position;
			public Vector3 rotation;
		}

		[SerializeField, Range(1, 6)]
		private int iterations = 0;

		[SerializeField]
		private float angle = 25f;

		[SerializeField]
		private TreeElement treeElement;

		[SerializeField]
		private string axiom = "X";

		[SerializeField]
		private float width = 1f;

		[SerializeField]
		private float minBranchSize = 0.5f;

		[SerializeField]
		private float maxBranchSize = 1f;

		[SerializeField]
		private float minLeafSize = 0.5f;

		[SerializeField]
		private float maxLeafSize = 1f;

		[SerializeField]
		private float variation = 10f;

		private Transform treeParent;

		private Vector3 posInit;

		private Vector3 rotInit;

		private Dictionary<char, string> rules = new Dictionary<char, string>();

		private string currenAxiom;

		private Stack<CachedTransform> transformStack = new Stack<CachedTransform>();

		public void SetWidth(float width)
		{
			this.width = width;
			GenerateAxiom();
		}

		public void SetIterations(float iterations)
		{
			this.iterations = (int)iterations;
			GenerateAxiom();
		}

		public void SetAngle(float angle)
		{
			this.angle = angle;
			GenerateAxiom();
		}

		public void SetVariation(float variation)
		{
			this.variation = variation;
			GenerateAxiom();
		}

		public void SetMinSizeBranch(float minSize)
		{
			this.minBranchSize = minSize;
			GenerateAxiom();
		}

		public void SetMaxSizeBranch(float maxSize)
		{
			this.maxBranchSize = maxSize;
			GenerateAxiom();
		}

		public void SetMinSizeLeaf(float minSize)
		{
			this.minLeafSize = minSize;
			GenerateAxiom();
		}

		public void SetMaxSizeLeaf(float maxSize)
		{
			this.maxLeafSize = maxSize;
			GenerateAxiom();
		}

		public void GenerateAxiom(bool cr = false)
		{
			transform.position = posInit;
			transform.eulerAngles = rotInit;

			rules.Clear();
			rules.Add('F', "FF");
			rules.Add('X', "F+[[X]-X]-F[-FX]+X");

			transformStack.Clear();

			if (treeParent != null)
				Destroy(treeParent.gameObject);

			treeParent = new GameObject("Tree Parent").transform;

			currenAxiom = axiom;

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

			StopAllCoroutines();

			if (cr == false)
				ShowTree();
			else
				StartCoroutine(ShowTreeCr());
		}

		public void ShowTree()
		{
			for (int i = 0; i < currenAxiom.Length; i++)
			{
				switch (currenAxiom[i])
				{
					case 'F':
						TreeElement aux = Instantiate(treeElement);
						aux.transform.SetParent(treeParent);
						aux.transform.position = transform.position;
						aux.lineRenderer.SetPosition(0, transform.position);
						aux.lineRenderer.widthMultiplier = width;

						Vector2 newPosition;

						if (currenAxiom[i + 1] % currenAxiom.Length == 'X' || currenAxiom[i + 3] % currenAxiom.Length == 'F' && currenAxiom[i + 4] % currenAxiom.Length == 'X')
						{
							newPosition = transform.position + transform.up * Random.Range(minLeafSize, maxLeafSize);
							aux.lineRenderer.material.color = Color.cyan;
							aux.lineRenderer.startWidth *= 2;
							aux.lineRenderer.endWidth = 0;
							aux.lineRenderer.sortingOrder = 1;
						}
						else
						{
							newPosition = transform.position + transform.up * Random.Range(minBranchSize, maxBranchSize);
						}

						aux.lineRenderer.SetPosition(1, newPosition);
						transform.position = newPosition;
						break;

					case '[':
						transformStack.Push(new CachedTransform
						{
							position = transform.position,
								rotation = transform.eulerAngles
						});
						break;

					case ']':
						CachedTransform cached = transformStack.Pop();
						transform.position = cached.position;
						transform.eulerAngles = cached.rotation;
						break;

					case '+':
						transform.Rotate(Vector3.forward * angle * (1 + variation / 100f * Random.Range(-1f, 1f)));
						break;

					case '-':
						transform.Rotate(Vector3.forward * -angle * (1 + variation / 100f * Random.Range(-1f, 0f)));
						break;
				}
			}
		}

		public IEnumerator ShowTreeCr()
		{
			Debug.Log(currenAxiom);

			for (int i = 0; i < currenAxiom.Length; i++)
			{
				switch (currenAxiom[i])
				{
					case 'F':
						TreeElement aux = Instantiate(treeElement);
						aux.transform.SetParent(treeParent);
						aux.transform.position = transform.position;
						aux.lineRenderer.SetPosition(0, transform.position);
						aux.lineRenderer.widthMultiplier = width;

						Vector2 newPosition;

						if (currenAxiom[i + 1] % currenAxiom.Length == 'X' || currenAxiom[i + 3] % currenAxiom.Length == 'F' && currenAxiom[i + 4] % currenAxiom.Length == 'X')
						{
							newPosition = transform.position + transform.up * Random.Range(minLeafSize, maxLeafSize);
							aux.lineRenderer.material.color = Color.cyan;
							aux.lineRenderer.startWidth *= 2;
							aux.lineRenderer.endWidth = 0;
							aux.lineRenderer.sortingOrder = 1;
						}
						else
						{
							newPosition = transform.position + transform.up * Random.Range(minBranchSize, maxBranchSize);
						}

						aux.lineRenderer.SetPosition(1, newPosition);
						transform.position = newPosition;

						yield return new WaitForEndOfFrame();
						break;

					case '[':
						transformStack.Push(new CachedTransform
						{
							position = transform.position,
								rotation = transform.eulerAngles
						});
						break;

					case ']':
						CachedTransform cached = transformStack.Pop();
						transform.position = cached.position;
						transform.eulerAngles = cached.rotation;
						break;

					case '+':
						transform.Rotate(Vector3.forward * angle * (1 + variation / 100f * Random.Range(-1f, 1f)));
						break;

					case '-':
						transform.Rotate(Vector3.forward * -angle * (1 + variation / 100f * Random.Range(-1f, 0f)));
						break;
				}
			}
		}

		public void Awake()
		{
			posInit = transform.position;
			rotInit = transform.eulerAngles;
			GenerateAxiom(true);
		}
	}