# line-it
Mobile game inspired by graph theory based on a Japanese game called Hashiwokakero (eng. Bridges).
Used a custom simple generation algorithm and DFS for validating the solution when the user connects all the nodes.
Each level generates a new random graph.

<h3>RULES</h3> 
<ul>
  <li>Connect the nodes with corresponding number of the lines</li>
  <li>Nodes should be connected with a single or double line</li>
  <li>All nodes must be in a single connected group</li>
</ul>

<br>
<div>
  <img src="https://user-images.githubusercontent.com/63672480/118521403-4b58db00-b73b-11eb-93a1-da48405251c9.jpg" alt="Main Menu" width="24%">
  <img src="https://user-images.githubusercontent.com/63672480/118522055-ee115980-b73b-11eb-9a08-d5100bc5408f.jpg" alt="Level" width="24%">
  <img src="https://user-images.githubusercontent.com/63672480/118522220-1b5e0780-b73c-11eb-8adb-c303e09ab043.jpg" alt="Level passed" width="24%">
  <img src="https://user-images.githubusercontent.com/63672480/218614903-92f90b18-5002-46ac-ae6b-69d0a153bc55.jpg" alt="invalid solution" width="24%">
  <br>
  <p>Image number:</p>
  <ol>
    <li>Main Menu</li>
    <li>Level example</li>
    <li>UI upon connecting all of the nodes correctly</li>
    <li>This solution is invalid as it violates the third rule</li>
  </ol>
</div>

<h3>BUGS</h3> 
<ul>
  <li>Unlimited connections allowed (more than 2, but only 3 rendered)</li>
  <li>Sometimes Unity Line Renderer renders the line in a random position connecting the node to nothing</li>
  <br>
  <p>Examples of bugs:</p>
  <img src="https://user-images.githubusercontent.com/63672480/218618561-f87e1bea-dd8a-4a49-9d20-22a31f9ae58f.jpg" alt="1st bug" width="24%">
  <img src="https://user-images.githubusercontent.com/63672480/218618576-0d1f5acb-2814-48d0-b7ec-1aa08a9c7a27.jpg" alt="2nd bug" width="24%">
</ul>
