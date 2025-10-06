import { useEffect, useState, Fragment } from 'react'
import './App.css'

type CinemaInfo = { title:string; rows:number; seatsPerRow:number; availableSeats:number; };
type Grid = boolean[][];

export default function App() {
  const [cinemaInfo, setCinemaInfo] = useState<CinemaInfo|null>(null);
  const [grid, setGrid] = useState<Grid>([]);
  const [seatCount, setSeatCount] = useState<number>(1);
  const [bookingId, setBookingId] = useState<string>("");
  const [selectedSeats, setSelectedSeats] = useState<Set<string>>(new Set());
  const [view, setView] = useState<'menu'|'book'|'check'>('menu');
  const [bookingLookupId, setBookingLookupId] = useState<string>("");

  // cinema form state
  const [title, setTitle] = useState<string>("Inception");
  const [rows, setRows] = useState<number>(8);
  const [seatsPerRow, setSeatsPerRow] = useState<number>(10);
  const [errors, setErrors] = useState<string[]>([]);

  // from-position input
  const [startSeat, setStartSeat] = useState<string>("");

  useEffect(() => {
    refreshCinemaInfo();
  }, [])

  async function refreshCinemaInfo() {
    const cinema = await fetch("/cinema").then(res => res.ok ? res.json() : null);
    setCinemaInfo(cinema);
    
    if (cinema) {
      const cinemaLayout = await fetch("/cinemalayout").then(res => res.ok ? res.json() : null);
      setGrid(cinemaLayout);
    } else {
      setGrid([]);
    }
  }

  async function readError(res: Response) {
    const txt = await res.text();
    try {
      const json = JSON.parse(txt);
      if (Array.isArray(json?.errors)) return json.errors.join("\n");
      if (typeof json?.error === "string") return json.error;
    } catch {}
    return txt;
  }

  function ErrorPanel({ items }: { items: string[] }) {
    if (!items || items.length === 0) return null;
    return (
      <div style={{ marginTop: 12, color: "crimson" }}>
        {items.map((e, i) => <div key={i}>• {e}</div>)}
      </div>
    );
  }

  async function createCinema() {
    setErrors([]);
    if (!title.trim()) { setErrors(["Title cannot be empty"]); return; }
    if (!Number.isInteger(rows) || rows < 1 || rows > 26) { setErrors(["Rows must be between 1 and 26"]); return; }
    if (!Number.isInteger(seatsPerRow) || seatsPerRow < 1 || seatsPerRow > 50) { setErrors(["Seats per row must be between 1 and 50"]); return; }

    // create cinema endpoint
    const createCinemaRes = await fetch("/cinema", {
      method: "POST",
      headers: {"Content-Type":"application/json"},
      body: JSON.stringify({ title, rows, seatsPerRow })
    });

    // creation issue, set list of errors
    if (!createCinemaRes.ok) {
      const msg = await readError(createCinemaRes);
      setErrors([msg || "Unknown error"]);
      return;
    }

    // creation success, reset errors and refresh cinema info
    setErrors([]);
    await refreshCinemaInfo();
    setView('menu');
  }

  async function startBookingAutoFill() {
    if (!Number.isInteger(seatCount) || seatCount < 1) { setErrors(["Seat number must be at least 1"]); return; }

    // preview default booking endpoint
    const bookingDetails = await fetch("/bookings", {
      method: "POST",
      headers: {"Content-Type":"application/json"},
      body: JSON.stringify({seatCount})
    });

    if (!bookingDetails.ok) {
      setErrors([await readError(bookingDetails)]);
      return;
    } else {
      const data = await bookingDetails.json();
      setErrors([]);
      setBookingId(data.bookingId);
      setSelectedSeats(new Set(data.seats)) // we use set because the seats should be unique
    }
  }

  async function startBookingFromPosition() {
    if (!startSeat.trim()) { setErrors(["Start seat is required"]); return; }

    const m = startSeat.trim().toUpperCase().match(/^([A-Z])(\d{2,})$/);
    if (!m) { setErrors(["Invalid seat format. Use like B03."]); return; }

    const col = parseInt(m[2], 10);
    if (col < 1 || col > (cinemaInfo?.seatsPerRow ?? 50)) { setErrors(["Column is out of range"]); return; }
    
    const rowIdx = m[1].charCodeAt(0) - 65;
    if (rowIdx < 0 || rowIdx >= (cinemaInfo?.rows ?? 26)) { setErrors(["Row is out of range"]); return; }

    // preview selected seat booking endpoint
    const bookingDetails = await fetch("/bookings/from-position", {
      method: "POST",
      headers: {"Content-Type":"application/json"},
      body: JSON.stringify({ seatCount, startSeat: startSeat.trim() })
    });

    if (!bookingDetails.ok) {
      setErrors([await readError(bookingDetails)]);
      return;
    } else {
      const data = await bookingDetails.json();
      setErrors([]);
      setBookingId(data.bookingId);
      setSelectedSeats(new Set(data.seats)) // we use set because the seats should be unique
    }
  }

  async function confirmSeats() {
    if (!bookingId || selectedSeats.size === 0) return;

    const seats = Array.from(selectedSeats)

    // confirm booking endpoint
    const confirmBookingDetails = await fetch(`/bookings/${bookingId}/confirm`, {
      method: "PUT",
      headers: {"Content-Type":"application/json"},
      body: JSON.stringify({seats})
    });

    // booking success, reset and refresh cinema layout
    if (confirmBookingDetails.ok) {
      setErrors([]);
      setBookingId("");
      setSelectedSeats(new Set());
      await refreshCinemaInfo();
    } else {
      setErrors([await readError(confirmBookingDetails)]);
    }
  }

  function seatCode (row: number, col: number) {
    return `${String.fromCharCode(65 + row )}${( col + 1).toString().padStart(2,'0')}`;
  }

  async function loadBookingSeats() {
    const id = bookingLookupId.trim().toUpperCase();
    if (!id) return;
    const res = await fetch(`/bookings/${id}`);
    if (res.status === 404) {
      setErrors(["Booking not found."]);
      return;
    }
    if (!res.ok) {
      setErrors([await readError(res)]);
      return;
    }
    const data = await res.json();
    setErrors([]);
    setSelectedSeats(new Set(data.seats));
  }

  async function resetCinema() {
    // clear server-side state and go back to create cinema screen
    await fetch("/reset", { method: "POST" }).catch(()=>{});
    setCinemaInfo(null);
    setGrid([]);
    setSelectedSeats(new Set());
    setBookingId("");
    setStartSeat("");
    setSeatCount(1);
    setErrors([]);
    setView('menu');
  }

  // --------------- UI ---------------

  // Create Cinema Form
  if (!cinemaInfo) {
    return (
      <div style={{padding: 16, fontFamily: "system-ui", maxWidth: 980, margin: "0 auto"}}>
        <h2>Create Cinema</h2>
        <div style={{display: "grid", gridTemplateColumns: "160px 1fr", gap: 12, alignItems: "center"}}>
          <label>Title</label>
          <input value={title} onChange={e=>setTitle(e.target.value)} />

          <label>Rows (1–26)</label>
          <input 
            type="number" 
            value={rows} 
            min={1} 
            max={26} 
            onChange={e=>setRows(parseInt(e.target.value||"0"))} 
          />
          <label>Seats / Row (1–50)</label>
          <input 
            type="number" 
            value={seatsPerRow} 
            min={1} 
            max={50} 
            onChange={e=>setSeatsPerRow(parseInt(e.target.value||"0"))} 
          />
        </div>

        <ErrorPanel items={errors} />

        <div style={{marginTop: 16}}>
          <button onClick={createCinema}>Create</button>
        </div>
      </div>
    );
  }

  // Main flow
  return (
    <div style={{padding: 16, fontFamily: "system-ui"}}>
      <h2
        title={cinemaInfo.title}
        style={{
          maxWidth: 800,
          display: "-webkit-box",
          WebkitLineClamp: 2,
          WebkitBoxOrient: "vertical",
          overflow: "hidden"
        }}
      >
        GIC Cinemas – {cinemaInfo.title}
      </h2>
      <div>Rows: {cinemaInfo.rows} · Seats/Row: {cinemaInfo.seatsPerRow} · Available: {cinemaInfo.availableSeats}</div>
      <ErrorPanel items={errors} />

      {/* MENU */}
      {view === 'menu' && (
        <div style={{marginTop: 16, display:"flex", gap:8, flexWrap:"wrap", justifyContent:"center"}}>
          <button onClick={()=>{ setErrors([]); setSelectedSeats(new Set()); setBookingId(""); setStartSeat(""); setSeatCount(1); setView('book'); }}>
            [1] Book tickets
          </button>
          <button onClick={()=>{ setErrors([]); setSelectedSeats(new Set()); setBookingId(""); setBookingLookupId(""); setView('check'); }}>
            [2] Check bookings
          </button>
          <button onClick={resetCinema}>
            [3] Reset
          </button>
        </div>
      )}

      {/* BOOKING */}
      {view === 'book' && (
        <div style={{marginTop: 16, display:"grid", gap:12, justifyItems:"center"}}>
          <div style={{display:"flex", gap:8, alignItems:"center", flexWrap:"wrap"}}>
            <label>Enter number of tickets to book:</label>
            <input 
              type="number" 
              min={1} 
              value={seatCount} 
              onChange={e=>setSeatCount(parseInt(e.target.value||"1"))}
              style={{width:120}}
            />
            <button onClick={startBookingAutoFill}>Auto-fill Preview</button>
          </div>
          <div style={{display:"flex", gap:8, alignItems:"center", flexWrap:"wrap"}}>
            <label>Or start seat (e.g. B03):</label>
            <input 
              placeholder="B03" 
              value={startSeat} 
              onChange={e=>setStartSeat(e.target.value)} 
              style={{width:120}}
            />
            <button onClick={startBookingFromPosition}>From Position Preview</button>
          </div>
          {bookingId && (
            <div style={{ marginTop: 8, fontSize: 14, color: "#333" }}>
              Preview booking id: <strong>{bookingId}</strong>
            </div>
          )}
          <div style={{display:"flex", gap:8}}>
            <button onClick={()=>{ setErrors([]); setSelectedSeats(new Set()); setBookingId(""); setStartSeat(""); setSeatCount(1); setView('menu') }}>Back to menu</button>
            <button onClick={confirmSeats} disabled={!bookingId}>Confirm</button>
          </div>
        </div>
      )}

      {/* CHECK BOOKING */}
      {view === 'check' && (
        <div style={{marginTop:16, display:"grid", gap:12}}>
          <div style={{display:"flex", gap:8, alignItems:"center", flexWrap:"wrap"}}>
            <label>Enter booking id:</label>
            <input value={bookingLookupId} onChange={e=>setBookingLookupId(e.target.value)} placeholder="GIC0001" style={{width:140}}/>
            <button onClick={loadBookingSeats}>Show</button>
            <button onClick={()=>{ setSelectedSeats(new Set()); setView('menu'); }}>Back to menu</button>
          </div>
        </div>
      )}

      {/* CINEMA LAYOUT */}
      <div style={{ display: "grid", gridTemplateColumns: `repeat(${cinemaInfo.seatsPerRow + 1}, 36px)`, gap: 6, marginTop: 16, justifyContent: "center" }}>
        {Array.from({ length: cinemaInfo.rows }, (_, i) => {
          const r = cinemaInfo.rows - 1 - i; // descending row index
          const rowLabel = String.fromCharCode(65 + r);
          return (
            <Fragment key={rowLabel}>
              <div style={{ textAlign: "center" }}>{rowLabel}</div>
              {Array.from({ length: cinemaInfo.seatsPerRow }, (_, c) => {
                const occupied = grid?.[r]?.[c] ?? false;
                const id = seatCode(r, c);
                const isSelected = selectedSeats.has(id);
                return (
                  <button
                    key={id}
                    disabled={true} // disable for now
                    style={{
                      padding: "6px 0",
                      borderRadius: 6,
                      border: isSelected ? "2px solid #2e7d32" : "1px solid #ccc",
                      background: isSelected ? "#e8f5e9" : "transparent",
                      opacity: occupied && !isSelected ? 0.5 : 1,
                      fontWeight: isSelected ? 700 : 400
                    }}
                  >
                    {isSelected ? "o" : occupied ? "#" : "."}
                  </button>
                );
              })}
            </Fragment>
          )
        })}

        <div />

        {Array.from({ length: cinemaInfo.seatsPerRow }, (_, c) => (
          <div key={`num-${c}`} style={{ textAlign: "center" }}>
            {c + 1}
          </div>
        ))}

      </div>
    </div>
  )
  
}
