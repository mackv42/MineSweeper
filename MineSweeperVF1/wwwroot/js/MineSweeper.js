var data;

function createBoardDOM( data ){
	let boardDiv = document.getElementById("board");
	for(let i=0; i<data.length; i++){
		let currentRow = createRow( "r" + i );
		for(let j=0; j<data[i].length; j++){
			let currentCell = createCell(i, j, data[i][j]);
			currentRow.appendChild(currentCell);
		}
		boardDiv.appendChild(currentRow);
	}
}

function clear( id ){
	document.getElementById( id ).innerHTML = "";
}

function createRow( name ){
	let row = document.createElement("div");
	row.classList.add("row");
	row.setAttribute("id", name);
	row.setAttribute("value", name);
	return row;
}

function log( e ){
	console.log(e.value);
}

function newGame(width, height, bombs){
	//ajax here
	//clear board, recreate board on response success
    var ret;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
           // console.log(Json.Parse(this.responseText));
            clear('board');
            data = JSON.parse(this.responseText);
            createBoardDOM(JSON.parse(this.responseText));

        }
    }
    xhttp.open("GET", "/MineSweeper/GenerateBoard?width="+width+"&height="+height+"&numBombs="+bombs, true)
    xhttp.send();
}

function ClickCell(x) {
    let coordinates = x.value.split("-");
    console.log(x.value);
    console.log(data);
    $.post(
        "/MineSweeper/Click",
        { board: data, x: parseInt(coordinates[1], 10), y:parseInt(coordinates[0], 10) },
        function (d) {
            clear("board");
            data = d;
            createBoardDOM(d);

        }
    );
}

function createCell( row, column, data ){
	let cell = document.createElement("div");
	cell.classList.add("c"+column);
	cell.classList.add("cell");
	let btn = createButton(row, column, data);
	/***change to click request as function when finished***/
	cell.appendChild(createButton(row, column, data, ClickCell));
	return cell; 
}


function createButton(row, column, data, click){
    let btn = document.createElement("BUTTON");

    if (data.hidden) {
        btn.innerHTML = "?";
    } else if (data.isBomb) {
        btn.innerHTML = "?";
    } else {
        btn.innerHTML = data.value;
    }

	btn.setAttribute("value", row+ "-"+column);
	//btn.innerHTML = data;
	
	btn.addEventListener("click", function(){click( this )})
	
	return btn;
}

createBoardDOM(board);

//creates a 10X10 board with 15 bombs
newGame(10, 10, 15);