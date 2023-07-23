import React, { Component } from 'react';
import './Home.css';
import Search from './Search';

export class Home extends Component {
  static displayName = Home.name;
  
    constructor(props) {
      super(props);
      this.state = { schedules: [], loading: true };
    }
  
    componentDidMount() {
      const filters = document.location.pathname //TODO: I SHOULD DO THIS WITH THE ROUTER I GUESS
      this.fetchData(filters);
    }
  
    render() {
      return (
          <div className='row'>
            <div className='col-9 horizontal-scrollable'>
              <div className="table-container">
                  <table className="table table-sm table-bordered">
                    <tbody>
                      {this.state.schedules.map((row, rowIndex) => (
                        <tr key={rowIndex}>
                          {row.map((cellValue, columnIndex) => (
                            <td key={`${rowIndex}-${columnIndex}`} className={cellValue}>{cellValue}</td>
                          ))}
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
            </div>
            <div className='col-3'>
              <Search />
            </div>
          </div>
      );
    }
  
    async fetchData(filters) {
      console.log(filters);
      const response = await fetch(`/api/schedules/table/${filters}`);
      const data = await response.json();
      console.log(data);
      this.setState({ schedules: data, loading: false });
    }
  }