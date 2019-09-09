CREATE TABLE Routes
(  
  Airline character varying(2) REFERENCES Airlines(TwoDigitCode),
  Origin character varying(3) REFERENCES Airports(Iata3),
  Destination character varying(3) REFERENCES Airports(Iata3)
);
