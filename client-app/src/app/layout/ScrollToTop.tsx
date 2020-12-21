import {useEffect} from 'react';
import {withRouter} from 'react-router-dom';

//https://reactrouter.com/web/guides/scroll-restoration
const ScrollToTop = ({ children, location: { pathname } }: any) => {
    useEffect(() => {
      window.scrollTo(0, 0);
    }, [pathname]);
  
    return children || null;
  };
  
  export default withRouter(ScrollToTop);