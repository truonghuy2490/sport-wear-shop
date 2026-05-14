import { BrowserRouter } from 'react-router-dom';
import AppRouter from './routes/AppRouter';
import ToastContainer from "./components/common/ToastContainer";

function App() {
  return (
    <BrowserRouter>
      <ToastContainer />
      <AppRouter />
    </BrowserRouter>
  );
}

export default App;