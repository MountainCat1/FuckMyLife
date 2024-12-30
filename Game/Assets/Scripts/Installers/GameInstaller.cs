using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            
            Container.Bind<IPathfinding>().To<Pathfinding>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IInputManager>().To<InputManager>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<IInputMapper>().To<InputMapper>().FromComponentsInHierarchy().AsSingle();
            
            
        }
    }
}