import FetchWrapper from "./ApiWrapper.js";
import { gameId, setLegalMoves} from "./GameState.js";


const fetchAPI = new FetchWrapper("http://localhost:5098/");

export async function updateLegalMoves() {
    try {
        const moves = await fetchAPI.post("GetLegalMoves", {
            gameId: gameId,
        });
        setLegalMoves(moves);
    } catch (error) {
        console.error("Error updating legal moves: " + error);
    }
}

export async function checkEndGame() {
    try {
        const result = await fetchAPI.post("CheckEndGame", { gameId });
        if (result.type != 0) {
            const modal = document.getElementById("endgame-modal");
            const title = document.getElementById("endgame-title");
            const message = document.getElementById("endgame-message");

            if (result.type === 1) {
                title.innerText = "Checkmate!";
                message.innerText = `Winner: ${result.winner === "w" ? "White" : "Black"}`;
            } else if (result.type === 2) {
                title.innerText = "Stalemate!";
                message.innerText = "The game ended in a draw.";
            } else if (result.type === 3) {
                title.innerText = "Draw!";
                message.innerText = "Insufficient material to continue.";
            }

            modal.classList.remove("hidden");
            document.getElementById("overlay-blocker").style.display = "block";
        } else {
            document.getElementById("overlay-blocker").style.display = "none";
        }

    } catch (error) {
        console.error("Error checking end game status: ", error);
    }
}