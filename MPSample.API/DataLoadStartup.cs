using MPSample.Common;
using MPSample.Domain.Entities;
using MPSample.Infrastructure.Repositories;
using System.Linq;

namespace MPSample.API
{
    public class DataLoadStartup
    {
        
        private readonly UnitOfWork _unitOfWork;
        public DataLoadStartup(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            
        }

        public void Load()
        {
            var teslaUser = new User { UserName = "tesla", Password = "tesla".ComputeSha256Hash() };
            var remaUser = new User { UserName = "rema", Password = "rema".ComputeSha256Hash() };
                var mcdonaldUser = new User { UserName = "mcdonald", Password = "mcdonald".ComputeSha256Hash() };

            var currentUsers = _unitOfWork.Users.GetAll();
            if (!currentUsers.Any(x => x.UserName == teslaUser.UserName))
                _unitOfWork.Users.Add(teslaUser);

            if (!currentUsers.Any(x => x.UserName == remaUser.UserName))
                _unitOfWork.Users.Add(remaUser);

            if (!currentUsers.Any(x => x.UserName == mcdonaldUser.UserName))
                _unitOfWork.Users.Add(mcdonaldUser);

            _unitOfWork.Commit();
        }
    }
}
