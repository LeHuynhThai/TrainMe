import React from 'react';
import Header from './Header';
import WorkoutSchedule from './WorkoutSchedule';

const WorkoutPage = () => {
  return (
    <div className="App">
      <Header />
      <div className="main-content">
        <WorkoutSchedule />
      </div>
    </div>
  );
};

export default WorkoutPage;
