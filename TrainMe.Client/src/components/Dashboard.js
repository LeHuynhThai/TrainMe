import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import Header from './Header';

const Dashboard = () => {
  const { user } = useAuth();

  return (
    <div className="App">
      <Header />

      <div className="main-content">
        <div className="dashboard-container fade-in">
          {/* Welcome Card */}
          <div className="welcome-card">
            <h1>Chào mừng đến với TrainMe!</h1>
            <p>Xin chào <strong>{user?.userName}</strong>, chúc bạn có một ngày tốt lành!</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
