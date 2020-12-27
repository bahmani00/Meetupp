import React, { useRef } from 'react';
import Cropper from 'react-cropper';
import 'cropperjs/dist/cropper.css';

const PhotoWidgetCropper: React.FC = () => {
  const cropper = useRef<Cropper>(null);

  const cropImage = () => {
    if (
      cropper.current &&
      typeof cropper.current.getCroppedCanvas() === 'undefined'
    ) {
      return;
    }
    cropper &&
      cropper.current &&
      cropper.current.getCroppedCanvas().toBlob((blob: any) => {
        //setImage(blob);
      }, 'image/jpeg');
  };

  return (
    <Cropper
      src="https://raw.githubusercontent.com/roadmanfong/react-cropper/master/example/img/child.jpg"
      style={{ height: 200, width: '100%' }}
      // Cropper.js options
      initialAspectRatio={16 / 9}
      preview='.img-preview' 
      guides={false}
      viewMode={1}
      dragMode='move'
      scalable={true}
      cropBoxMovable={true}
      cropBoxResizable={true}
      crop={cropImage}
    />
  );
};

export default PhotoWidgetCropper;
