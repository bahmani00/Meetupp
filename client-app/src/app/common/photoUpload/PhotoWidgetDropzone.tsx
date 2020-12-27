import React, { useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { Icon, Header } from 'semantic-ui-react'; 

const dropzoneStyles = {
  border: 'dashed 3px',
  borderColor: '#eee',
  borderRadius: '5px',
  paddingTop: '30px',
  textAlign: 'center' as 'center',
  height: '200px'
};

const dropzoneActive = {
  borderColor: 'green'
};

const PhotoWidgetDropzone = () => {
  //when a file dropped this even get raised
  const onDrop = useCallback(acceptedFiles => {
    console.log(acceptedFiles);
  }, []);
  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

  return (
    <div
      {...getRootProps()}
      style={
        isDragActive ? { ...dropzoneStyles, ...dropzoneActive } : dropzoneStyles
      }
    >
      <input {...getInputProps()} />
      <Icon name='upload' size='huge' />
      <Header content='Drop image here' />
    </div>
  );
};

export default PhotoWidgetDropzone;
