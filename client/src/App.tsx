import './App.css';
import ReviewList from '@/components/reviews/review-list.tsx';

function App() {
   return (
      <div className="p-4 h-screen w-full">
         <ReviewList productId={4} />
      </div>
   );
}

export default App;
