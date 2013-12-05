param($installPath, $toolsPath, $package, $project)

$frameworkMoniker = $project.Properties.Item("TargetFrameworkMoniker").Value

$framework = new-object System.Runtime.Versioning.FrameworkName($frameworkMoniker)

if ($framework.Identifier -ne '.NetCore')
{
	install-package 'CommonServiceLocator'
}

$project.DTE.ItemOperations.Navigate('http://unity.codeplex.com/')